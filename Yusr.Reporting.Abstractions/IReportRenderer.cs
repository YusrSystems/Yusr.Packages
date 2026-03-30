namespace Yusr.Reporting.Abstractions
{
    public interface IReportRenderer<TData> where TData : BaseRendererData
    {
        public byte[] Render(TData data);
    }
}
