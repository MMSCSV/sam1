namespace CareFusion.Dispensing.Data
{
    public interface IContractConvertible<TContract>
    {
        TContract ToContract();
    }
}
