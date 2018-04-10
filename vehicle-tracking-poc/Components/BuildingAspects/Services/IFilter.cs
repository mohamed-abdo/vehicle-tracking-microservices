using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingAspects.Services
{
    public interface IFilter
    {
        long StartFromTime { get; }
        long EndByTime { get; }
        string SortBy { get; }
        int PageSize { get; }
        int PageNo { get; }
    }
}
