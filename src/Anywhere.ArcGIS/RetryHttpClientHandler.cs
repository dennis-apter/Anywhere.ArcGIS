using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Anywhere.ArcGIS
{
    public class RetryHttpClientHandler : HttpClientHandler
    {
        public static readonly int DefaultClientRetryCount = 10;

        public RetryHttpClientHandler()
            : this(10)
        {
        }

        public RetryHttpClientHandler(int retryCount)
        {
            ArgumentNotNegativeValue(retryCount, nameof(retryCount));

            RetryCount = retryCount;
        }

        public event EventHandler<RetryingEventArgs> Retrying;

        protected int RetryCount { get; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            int retryCount = 0;
            var delayBeforeRetry = TimeSpan.Zero;

            while (true)
            {
                Exception lastException;
                try
                {
                    var responseMessage = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                    if (IsSuccess(responseMessage))
                    {
                        return responseMessage;
                    }

                    lastException = null;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    if (!IsTransient(ex))
                    {
                        throw;
                    }

                    if (!ShouldRetry(retryCount++, ex, ref delayBeforeRetry))
                    {
                        throw;
                    }
                }

                if (delayBeforeRetry.TotalMilliseconds < 0)
                {
                    delayBeforeRetry = TimeSpan.Zero;
                }

                Retrying?.Invoke(this, new RetryingEventArgs(retryCount, delayBeforeRetry, lastException));

                if (retryCount > 1 && delayBeforeRetry.TotalMilliseconds > 0)
                {
                    await Task.Delay(delayBeforeRetry, cancellationToken);
                }
            }
        }

        protected virtual bool IsSuccess(HttpResponseMessage responseMessage)
        {
            return responseMessage.IsSuccessStatusCode;
        }

        protected virtual bool IsTransient(Exception lastException)
        {
            return false;
        }

        protected virtual bool ShouldRetry(int currentRetryCount, Exception lastException,
            ref TimeSpan delayBeforeRetry)
        {
            return currentRetryCount < RetryCount;
        }

        internal static void ArgumentNotNegativeValue(int argumentValue, string argumentName)
        {
            if (argumentValue < 0)
            {
                throw new ArgumentOutOfRangeException(
                    argumentName, $"The specified argument {argumentName} cannot be initialized with a negative value.");
            }
        }

        internal static void ArgumentNotGreaterThan(double argumentValue, double ceilingValue, string argumentName)
        {
            if (argumentValue > ceilingValue)
            {
                throw new ArgumentOutOfRangeException(argumentName,
                    string.Format(CultureInfo.CurrentCulture,
                        "The specified argument {0} cannot be greater than its ceiling value of {1}.", argumentName, ceilingValue));
            }
        }
    }

    public class IncrementalRetryHttpClientHandler : RetryHttpClientHandler
    {
        private readonly TimeSpan _initialInterval;
        private readonly TimeSpan _increment;

        public IncrementalRetryHttpClientHandler()
            : this(DefaultClientRetryCount, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(1))
        {
        }

        public IncrementalRetryHttpClientHandler(int retryCount, TimeSpan initialInterval, TimeSpan increment)
            : base(retryCount)
        {
            ArgumentNotNegativeValue((int)initialInterval.Ticks, nameof(initialInterval));
            ArgumentNotNegativeValue((int)increment.Ticks, nameof(increment));

            _initialInterval = initialInterval;
            _increment = increment;
        }

        protected override bool ShouldRetry(int currentRetryCount, Exception lastException,
            ref TimeSpan delayBeforeRetry)
        {
            if (currentRetryCount < RetryCount)
            {
                delayBeforeRetry = TimeSpan.FromMilliseconds(
                    _initialInterval.TotalMilliseconds + _increment.TotalMilliseconds * currentRetryCount);

                return true;
            }

            return false;
        }
    }

    public class ExponentialBackoffRetryHttpClientHandler : RetryHttpClientHandler
    {
        public static readonly TimeSpan DefaultDeltaBackoff = TimeSpan.FromSeconds(10.0);
        public static readonly TimeSpan DefaultMaxBackoff = TimeSpan.FromSeconds(30.0);
        public static readonly TimeSpan DefaultMinBackoff = TimeSpan.FromSeconds(1.0);

        private readonly TimeSpan _minBackoff;
        private readonly TimeSpan _maxBackoff;
        private readonly TimeSpan _deltaBackoff;

        public ExponentialBackoffRetryHttpClientHandler()
            : this(DefaultClientRetryCount, DefaultMinBackoff, DefaultMaxBackoff, DefaultDeltaBackoff)
        {
        }

        public ExponentialBackoffRetryHttpClientHandler(int retryCount, TimeSpan minBackoff, TimeSpan maxBackoff, TimeSpan deltaBackoff)
            : base(retryCount)
        {
            ArgumentNotNegativeValue((int)minBackoff.Ticks, nameof(minBackoff));
            ArgumentNotNegativeValue((int)maxBackoff.Ticks, nameof(maxBackoff));
            ArgumentNotNegativeValue((int)deltaBackoff.Ticks, nameof(deltaBackoff));
            ArgumentNotGreaterThan(minBackoff.TotalMilliseconds, maxBackoff.TotalMilliseconds, nameof(minBackoff));

            _minBackoff = minBackoff;
            _maxBackoff = maxBackoff;
            _deltaBackoff = deltaBackoff;
        }

        protected override bool ShouldRetry(int currentRetryCount, Exception lastException,
            ref TimeSpan delayBeforeRetry)
        {
            if (currentRetryCount < RetryCount)
            {
                var random = new Random();

                var delta = (Math.Pow(2.0, currentRetryCount) - 1.0) *
                    random.Next(
                        (int)(_deltaBackoff.TotalMilliseconds * 0.8),
                        (int)(_deltaBackoff.TotalMilliseconds * 1.2));

                var interval = (int)Math.Min(_minBackoff.TotalMilliseconds + delta, _maxBackoff.TotalMilliseconds);
                delayBeforeRetry = TimeSpan.FromMilliseconds(interval);
                return true;
            }

            return false;
        }
    }

    public class RetryingEventArgs : EventArgs
    {
        public RetryingEventArgs(int currentRetryCount, TimeSpan delay, Exception lastException)
        {
            CurrentRetryCount = currentRetryCount;
            Delay = delay;
            LastException = lastException;
        }

        public int CurrentRetryCount { get; }
        public TimeSpan Delay { get; }
        public Exception LastException { get; }
    }
}
