namespace Infrastructure.Extentions
{
    public static class StringExtentions
    {
        public static byte[] ToByteArray(this string str)
        {
            return System.Text.Encoding.ASCII.GetBytes(str);
        }
    }
}
