namespace EtoFormsUI
{
    public class TextBoxDefinition
    {
        /// <summary>
        /// The row of the popup to show on
        /// </summary>
        public int index { get; set; }
        
        /// <summary>
        /// Name to identify the value
        /// </summary>
        public string Name { get; set; }
        
        public string InitialValue { get; set; }

        /// <summary>
        /// Minimum numeric value
        /// </summary>
        public int? MinValue { get; set; }
    }
}