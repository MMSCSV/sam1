using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing
{
	public interface IContextProvider
	{
		Context Context
		{
			get;
			set;
		}

        Context GetCurrentContext();
	}
}
