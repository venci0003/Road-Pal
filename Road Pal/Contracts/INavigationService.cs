using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadPal.Contracts
{
	public interface INavigationService
	{
		Task NavigateToPage(Page page);
		Task GoBack();

		Task RefreshCurrentPage(Page page);
	}
}
