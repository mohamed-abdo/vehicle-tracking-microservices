namespace BuildingAspects.Services
{
    public class FilterCritieria : IFilter
    {
        private readonly long _startFromTime;
        private readonly long _endByTime;
        private readonly string _sortBy;
        private readonly int _pageSize;
        private readonly int _pageNo;

        public FilterCritieria(long startFromTime, long endByTime, string sortBy, int pageSize, int pageNo)
        {
            _startFromTime = startFromTime;
            _endByTime = endByTime;
            _sortBy = sortBy;
            _pageSize = pageSize;
            _pageNo = pageNo;
        }
        public long StartFromTime { get => _startFromTime; }
        public long EndByTime { get => _endByTime; }
        public string SortBy { get => _sortBy; }
        public int PageSize { get => _pageSize; }
        public int PageNo { get => _pageNo; }
    }
}
