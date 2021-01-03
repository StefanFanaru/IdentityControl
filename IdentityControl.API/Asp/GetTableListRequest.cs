namespace IdentityControl.API.Asp
{
    public class GetTableListRequest<TFilter>
    {
        public int PageIndex { get; set; }
        public int PageLimit { get; set; }
        public string SortColumn { get; set; }
        public SortDirection SortDirection { get; set; }
        public TFilter FilterType { get; set; }
    }
}