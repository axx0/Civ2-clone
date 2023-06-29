namespace RaylibUI.Forms;

public struct Padding
{
    public int L;
    public int R;
    public int T;
    public int B;

    public Padding(int L, int R, int T, int B)
    {
        this.L = L;
        this.R = R;
        this.T = T;
        this.B = B;
    }

    public Padding(int border)  
    {
        L = R = T = B = border;
    }
}
