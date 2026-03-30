using Yusr.Core.Abstractions.Models;

namespace Yusr.Reporting.Abstractions
{
    public class BaseRendererData(ReportContext reportContext, byte[]? logoBytes)
    {
        public ReportContext ReportContext { get; set; } = reportContext;
        public byte[]? LogoBytes { get; set; } = logoBytes;
    }
}
