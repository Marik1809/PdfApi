using System.Collections.Generic;

namespace PdfApi.Model
{
    public class PdfFileListModel
    {
        public IEnumerable<PdfFileModel> Files { get; set; }
        public int Page { get; set; }
        public int ItemsPerPage { get; set; }
        public int FilesTotal { get; set; }
    }
}
