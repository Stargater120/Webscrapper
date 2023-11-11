namespace Webscrapper.CORE;

public class KeyStore
{
    private Byte[] _RSAKey;

    public Byte[] RSAKey
    {
        get
        {
            return _RSAKey;
        }
        set
        {
            _RSAKey = value;
        }
    }
}