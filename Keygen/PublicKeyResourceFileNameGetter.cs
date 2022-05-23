using System.Collections.Generic;
using System.Text;

public class PublicKeyResourceFileNameGetter
{
    public string Get()
    {
        List<int> ints = new List<int>();
        ints.Add(65);
        ints.Add(66);
        ints.Add(67);
        StringBuilder sb = new StringBuilder();
        foreach (int i in ints)
        {
            char b = (char)i;
            sb.Append(b);
        }
        return sb.ToString();
    }
}

