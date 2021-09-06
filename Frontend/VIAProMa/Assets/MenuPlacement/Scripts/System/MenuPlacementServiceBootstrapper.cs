using i5.Toolkit.Core.ServiceCore;
using MenuPlacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPlacementServiceBootstrapper : BaseServiceBootstrapper
{

    [SerializeField]private MenuPlacementService menuPlacementService;
    protected override void RegisterServices() {
        ServiceManager.RegisterService(menuPlacementService);
    }

    protected override void UnRegisterServices() {
        ServiceManager.RemoveService<MenuPlacementService>();
    }
}
