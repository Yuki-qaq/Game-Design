using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Kernel;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginMMO.NetSyncs
{
    /// <summary>
    /// 网络材质
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Material)]
    [DisallowMultipleComponent]
    [Name("网络材质")]
    [Tool(MMOHelperExtension.Title, nameof(NetIdentity), rootType = typeof(MMOManager))]
    public class NetMaterial : NetMB, IAwake
    {
        /// <summary>
        /// 渲染器
        /// </summary>
        [Name("渲染器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Renderer _renderer;

        /// <summary>
        /// 材质名
        /// </summary>
        [SyncVar]
        [Readonly]
        [Name("材质名")]
        public string _materialName;

        /// <summary>
        /// 之前材质名
        /// </summary>
        [Readonly]
        [Name("之前材质名")]
        public string _prevMaterialName;

        /// <summary>
        /// 原始材质
        /// </summary>
        [Readonly]
        [Name("原始材质")]
        public Material _originalMaterial;

        /// <summary>
        /// 唤醒：初始化
        /// </summary>
        public void Awake()
        {
            if(!_renderer)
            {
                _renderer = GetComponent<Renderer>();
                if (_renderer)
                {
                    // 记录以前的材质
                    UnityAssetObjectManager.instance.Add(_renderer.material);
                }
            }
        }

        /// <summary>
        /// 当MMO进入房间完成
        /// </summary>
        /// <param name="result"></param>
        public override void OnMMOEnterRoomCompleted(EACode result)
        {
            base.OnMMOEnterRoomCompleted(result);

            if (_renderer)
            {
                _originalMaterial = _renderer.material;
                if (_originalMaterial)
                {
                    _prevMaterialName = _materialName = _originalMaterial.name;
                }
            }
        }

        /// <summary>
        /// 当MMO退出房间完成
        /// </summary>
        public override void OnMMOExitRoomCompleted()
        {
            base.OnMMOExitRoomCompleted();
            if (_renderer)
            {
                _renderer.material = _originalMaterial;
            }
        }

        private string GetOriginalMaterialName(string matName)
        {
            int length = matName.LastIndexOf(" (Instance)");
            return length >= 0 ? matName.Substring(0, length) : matName;
        }

        /// <summary>
        /// 当定时检查修改
        /// </summary>
        /// <returns></returns>
        protected override bool OnTimedCheckChange()
        {
            if (_renderer && _renderer.material)
            {
                _materialName = GetOriginalMaterialName(_renderer.material.name);
            }
            return _materialName != _prevMaterialName;
        }

        /// <summary>
        /// 当同步变量已修改
        /// </summary>
        protected override void OnSyncVarChanged()
        {
            base.OnSyncVarChanged();
            if (_renderer)
            {
                if (!SetRenderMaterial(_materialName))
                {
                    var oriName = GetOriginalMaterialName(_materialName);
                    if (oriName != _materialName)
                    {
                        SetRenderMaterial(oriName);
                    }
                }

                _prevMaterialName = _materialName;
            }
        }

        private bool SetRenderMaterial(string matName)
        {
            Material newMat = UnityAssetObjectManager.instance.GetUnityAssetObject<Material>(matName);
            if (newMat)
            {
                _renderer.material = newMat;
                return true;
            }
            return false;
        }
    }
}
