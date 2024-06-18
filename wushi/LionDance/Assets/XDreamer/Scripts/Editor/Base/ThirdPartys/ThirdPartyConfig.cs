using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.Helper;
using XCSJ.Interfaces;
using XCSJ.Languages;

namespace XCSJ.EditorExtension.Base.ThirdPartys
{
    /// <summary>
    /// 第三方库配置
    /// </summary>
    public class ThirdPartyConfig
    {
        /// <summary>
        /// 配置列表
        /// </summary>
        public List<VersionConfig> configs = new List<VersionConfig>();

        /// <summary>
        /// 应用配置
        /// </summary>
        /// <param name="version">版本</param>
        /// <param name="workDirectory">工作目录</param>
        public void ApplyConfig(string version, string workDirectory)
        {
            foreach (var versionConfig in configs)
            {
                versionConfig.ApplyConfig(version, workDirectory);
            }
        }
    }

    /// <summary>
    /// 版本配置
    /// </summary>
    public class VersionConfig
    {
        /// <summary>
        /// 开始版本
        /// </summary>
        public string beginVersion = "";

        /// <summary>
        /// 结束版本
        /// </summary>
        public string endVersion = "";

        /// <summary>
        /// 文件配置列表
        /// </summary>
        public List<FileConfig> fileConfigs = new List<FileConfig>();

        /// <summary>
        /// 应用配置
        /// </summary>
        /// <param name="version">版本</param>
        /// <param name="workDirectory">工作目录</param>
        /// <returns></returns>
        public bool ApplyConfig(string version, string workDirectory)
        {
            if (EditorHelper.VersionValid(version, beginVersion, endVersion))
            {
                foreach (var fileConfig in fileConfigs)
                {
                    fileConfig.ApplyConfig(version, workDirectory);
                }
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 文件配置
    /// </summary>
    public class FileConfig
    {
        /// <summary>
        /// 文件
        /// </summary>
        public FileTuple file = new FileTuple();

        /// <summary>
        /// 元文件
        /// </summary>
        public FileTuple metaFile = new FileTuple();

        /// <summary>
        /// 应用配置
        /// </summary>
        /// <param name="version">版本</param>
        /// <param name="workDirectory">工作目录</param>
        public void ApplyConfig(string version, string workDirectory)
        {
            file.ApplyConfig(version, workDirectory);
            metaFile.ApplyConfig(version, workDirectory);
        }
    }

    /// <summary>
    /// 文件元组
    /// </summary>
    [LanguageFileOutput]
    public class FileTuple : IOnAfterDeserialize
    {
        /// <summary>
        /// 源
        /// </summary>
        public string src = "";

        /// <summary>
        /// 目标
        /// </summary>
        public string dst = "";

        /// <summary>
        /// 应用配置
        /// </summary>
        /// <param name="version">版本</param>
        /// <param name="workDirectory">工作目录</param>
        [LanguageTuple("The destination path must be a path relative to the current project asset folder!", "目标路径必须是相对当前工程资产文件夹的路径！")]
        [LanguageTuple("The destination directory does not exist:", "目标目录不存在：")]
        [LanguageTuple("Invalid working directory:", "无效工作目录：")]
        [LanguageTuple("Source file does not exist:", "源文件不存在：")]
        public void ApplyConfig(string version, string workDirectory)
        {
            //目标文件已存在不处理
            if (FileHelper.Exists(dst))
            {
                //Debug.Log("目标文件已存在：" + dst);
                return;
            }

            //确保目标文件路径的合理性
            if (!dst.StartsWith("Assets/"))
            {
                Debug.LogWarning("The destination path must be a path relative to the current project asset folder!".Tr(typeof(FileTuple)));
                return;
            }

            //确保目标文件所在目录存在
            var dstDir = Path.GetDirectoryName(dst);
            if (!Directory.Exists(dstDir))
            {
                Debug.LogWarning("The destination directory does not exist:".Tr(typeof(FileTuple)) + dstDir);
                return;
            }

            //确保源文件路径的合理性
            var tmpSrc = src;
            if (!tmpSrc.StartsWith("Assets/"))
            {
                workDirectory = workDirectory.Replace("\\", "/");
                if (!workDirectory.StartsWith("Assets/"))
                {
                    Debug.LogWarning("Invalid working directory:".Tr(typeof(FileTuple)) + workDirectory);
                    return;
                }
                tmpSrc = Path.Combine(workDirectory, src);
            }

            //确保源文件存在
            if (!FileHelper.Exists(tmpSrc))
            {
                Debug.LogWarning("Source file does not exist:".Tr(typeof(FileTuple)) + src);
                return;
            }

            //复制源文件内容到目标路径文件中
            var text = FileHelper.InputFile(tmpSrc, false, false);
            FileHelper.OutputFile(dst, text, false, false);

            //Debug.Log("复制文件内容：" + tmpSrc + "==>" + dst);
        }

        void IOnAfterDeserialize.OnAfterDeserialize(ISerializeContext serializeContext)
        {
            src = src.Replace("\\", "/");
            dst = dst.Replace("\\", "/");
        }
    }
}
