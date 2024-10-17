using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blecon.BGLocationTracking.Services;

public interface IPermissionService
{
    Task<PermissionStatus> CheckAndRequestLocationAlwaysPermission(bool showRationale = false);
}
