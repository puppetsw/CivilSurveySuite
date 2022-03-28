using System.Data;
using System.Text;

namespace _3DS_CivilSurveySuite.UI.Helpers
{
    public static class DataTableHelpers
    {
        /// <summary>
        /// Converts a DataView to delimited string.
        /// </summary>
        /// <param name="dataView">The <see cref="DataView"/>.</param>
        /// <param name="writeHeaders">Should column headers be written to string.</param>
        /// <param name="delimiter">The delimiter to use.</param>
        /// <returns>System.String.</returns>
        public static string ToCsv(this DataView dataView, bool writeHeaders = false, string delimiter = ",")
        {
            DataTable dt = dataView.ToTable();

            StringBuilder sb = new StringBuilder();

            if (writeHeaders)
            {
                // Write column headings.
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sb.Append(dt.Columns[i]);
                    if (i < dt.Columns.Count - 1)
                    {
                        sb.Append(delimiter);
                    }
                }

                sb.AppendLine();
            }

            // Write data
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    sb.Append(dt.Rows[i][j]);

                    if (j < dt.Columns.Count - 1)
                    {
                        sb.Append(delimiter);
                    }
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
