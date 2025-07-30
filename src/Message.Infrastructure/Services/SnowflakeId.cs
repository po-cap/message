namespace Message.Infrastructure.Services;

public class SnowflakeId
{
    private const long Twepoch = 1288834974657L; // 起始时间戳（2010-11-04 09:42:54.657 UTC）

    private const int WorkerIdBits = 5;
    private const int DatacenterIdBits = 5;
    private const int SequenceBits = 12;
    
    private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
    private const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);

    private const int WorkerIdShift = SequenceBits;
    private const int DatacenterIdShift = SequenceBits + WorkerIdBits;
    private const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;
    private const long SequenceMask = -1L ^ (-1L << SequenceBits);

    private long _sequence;
    private long _lastTimestamp = -1L;

    public SnowflakeId(long workerId, long datacenterId)
    {
        if (workerId > MaxWorkerId || workerId < 0)
        {
            throw new ArgumentException($"Worker ID 必须在 0 和 {MaxWorkerId} 之间");
        }

        if (datacenterId > MaxDatacenterId || datacenterId < 0)
        {
            throw new ArgumentException($"Datacenter ID 必须在 0 和 {MaxDatacenterId} 之间");
        }

        WorkerId = workerId;
        DatacenterId = datacenterId;
    }

    public long WorkerId { get; }
    public long DatacenterId { get; }

    public long Sequence
    {
        get { return _sequence; }
        internal set { _sequence = value; }
    }

    /// <summary>
    /// 取得 ID
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public long Get()
    {
        lock (this)
        {
            var timestamp = TimeGen();

            if (timestamp < _lastTimestamp)
            {
                throw new InvalidOperationException(
                    $"时钟回拨，拒绝生成 ID。{_lastTimestamp - timestamp} 毫秒");
            }

            if (_lastTimestamp == timestamp)
            {
                _sequence = (_sequence + 1) & SequenceMask;
                if (_sequence == 0)
                {
                    timestamp = TilNextMillis(_lastTimestamp);
                }
            }
            else
            {
                _sequence = 0L;
            }

            _lastTimestamp = timestamp;

            return ((timestamp - Twepoch) << TimestampLeftShift) |
                   (DatacenterId << DatacenterIdShift) |
                   (WorkerId << WorkerIdShift) |
                   _sequence;
        }
    }

    private long TilNextMillis(long lastTimestamp)
    {
        var timestamp = TimeGen();
        while (timestamp <= lastTimestamp)
        {
            timestamp = TimeGen();
        }
        return timestamp;
    }

    private long TimeGen()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}