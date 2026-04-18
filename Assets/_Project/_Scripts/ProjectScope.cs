using _Project._Scripts.Meta.Data;
using _Project._Scripts.Shared.Data;
using Reflex.Core;
using UnityEngine;

namespace _Project._Scripts
{
    public class ProjectScope : MonoBehaviour, IInstaller
    {
        [SerializeField] private StyleSheetRepository styleSheetRepository;
        
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterValue(styleSheetRepository);
        }
    }
}