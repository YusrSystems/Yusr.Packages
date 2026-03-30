namespace Yusr.Reporting.Abstractions
{
    public class ReportResult<TRendererData> where TRendererData : BaseRendererData
    {
        public byte[] ReportBytes { get; set; } = [];
        public TRendererData RendererData { get; set; }

        public ReportResult(byte[] reportBytes, TRendererData rendererData)
        {
            ReportBytes = reportBytes;
            RendererData = rendererData;
        }
    }
}
