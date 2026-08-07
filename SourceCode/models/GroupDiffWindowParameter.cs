namespace AdSnooperGui.models
{
    public class GroupDiffWindowParameter
    {
        /// <summary>
        /// Column header with Person-ID or Name
        /// </summary>
        public string [] columnHeader { get; set; }


        public double [] columnWidth { get; set; }

        /// <summary>
        /// Tabledata 2-Dimensional Array [Row,Column]
        /// </summary>
        public string [,] data { get; set; }
    }


}
