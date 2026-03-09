using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Events
{
    public record ProductPriceChangedIntegrationEvent(int ProductId, decimal NewPrice);
    
}
