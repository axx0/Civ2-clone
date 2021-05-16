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
        
        /// <summary>
        /// Force the box to have numeric values
        /// </summary>
        public bool Numeric { get; set; }

        public string InitialValue { get; set; }
    }
}