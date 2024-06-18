using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Base.Kernel;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginGenericStandards.Managers;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.Converters
{
    /// <summary>
    /// 字符串与纹理转换器
    /// </summary>
    [Name("字符串与纹理转换器")]
    [Tool(XGUICategory.DataConverter, nameof(BaseDataConverter))]
    public class String_Texture_Converter : BaseDataConverter, IDataConverter<string, Texture>
    {
        /// <summary>
        /// 默认纹理
        /// </summary>
        [Name("默认纹理")]
        public Texture _defaultTexture;

        BaseModelView sender;
        string url;

        static Dictionary<string, Texture> cache = new Dictionary<string, Texture>();

        /// <summary>
        /// 尝试转换到
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="input"></param>
        /// <param name="outputType"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public override bool TryConvertTo(BaseModelView sender, object input, Type outputType, out object output)
        {
            if(input is string i)
            {
                if (string.IsNullOrEmpty(i))
                {
                    output = default(Texture);
                    return true;
                }
                if (cache.TryGetValue(i, out var t) && t)
                {
                    output = t;
                    return true;
                }
                //网络路径
                if (PathHandler.IsHttpPath(i))
                {
                    //Debug.Log("IsHttpPath: " + path);
                    if (url != i)//新的URL请求
                    {
                        this.sender = sender;
                        this.url = i;
                        FileHandler.LoadHttpFile(i, EDataType.Texture, OnTextureLoaded, default);
                    }
                    else
                    {
                        output = _defaultTexture;
                        return true;
                    }
                }
                else
                {
                    var tex = UnityAssetObjectManager.instance.GetUnityAssetObject<Texture>(i);
                    if (tex)
                    {
                        cache[i] = tex;
                        output = tex;
                        return true;
                    }
                }
                output = _defaultTexture;
                return true;
            }
            return base.TryConvertTo(sender, input, outputType, out output);
        }

        private void OnTextureLoaded(IDataInfo dataInfo, object tag)
        {
            var tex = dataInfo.texture;
            if (!tex) return;

            var url = dataInfo.url;
            cache[url] = tex;
            if (url == this.url && sender)
            {
                sender.ModelToView();
                sender = null;
            }
        }
    }
}
