namespace AdSnooperGui.models
{
    public class DynamicDataTableColumn
    {
        /// <summary>
        /// Call with "Column1", "cellData[0]", 200
        /// </summary>
        /// <param name="header"></param>
        /// <param name="binding"></param>
        /// <param name="width"></param>
        public DynamicDataTableColumn(string header, string binding, double width)
        {
            this.header = header;
            this.binding = binding;
            this.width = width;
        }
        public string header { get; set; }
        public string binding { get; set; }
        public double width { get; set; }
    }

}
