namespace DomainModels.System
{
    public interface IFilter 
    {
        long StartFromTime { get; }
        long EndByTime { get; }
        //TODO:future feature
        //string SortBy { get; }
        int PageSize { get; }
        int PageNo { get; }
    }
}