using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions.Base;
using XCSJ.Extension.Base.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.Base;
using XCSJ.PluginTools.Inputs;

namespace XCSJ.PluginPhysicses.Tools.Weapons
{
    /// <summary>
    /// 发射器：模拟射击、换发射物容器与冷却
    /// </summary>
    [Name("发射器")]
    [RequireManager(typeof(PhysicsManager))]
    [Owner(typeof(PhysicsManager))]
    [Tool(PhysicsCategory.Title, rootType = typeof(PhysicsManager))]  
    public class Shooter : Interactor
    {
        /// <summary>
        /// 发射口：用于设定弹药发射的位置与角度
        /// </summary>
        [Name("发射口")]
        [Tip("用于设定弹药发射的位置与角度", "Used to set the position and angle of ammunition launch")]
        public RayGenerater _shootRay;

        /// <summary>
        /// 发射点位置
        /// </summary>
        public Ray ray => _shootRay.TryGetRay(out var r) ? r : default;

        /// <summary>
        /// 发射力
        /// </summary>
        [Name("发射力")]
        [Min(0)]
        [Tip("作用在发射物上的力", "Force acting on a projectile")]
        public float _power = 100f;

        /// <summary>
        /// 发射力
        /// </summary>
        public float power => _power;

        /// <summary>
        /// 发射音频剪辑
        /// </summary>
        [Name("发射音频剪辑")]
        public AudioClip _shootClip;

        /// <summary>
        /// 发射音频音量
        /// </summary>
        [Name("发射音频音量")]
        public float _shootVolume = 1f;

        /// <summary>
        /// 发射物容器
        /// </summary>
        [Name("发射物容器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public MissileContainer _missileContainer;

        /// <summary>
        /// 换发射物容器音频
        /// </summary>
        [Name("换发射物容器音频")]
        public AudioClip _changeMissileContainerAudioClip;

        /// <summary>
        /// 发射物容器
        /// </summary>
        public MissileContainer missileContainer
        {
            get => _missileContainer;
            set
            {
                if (_missileContainer != value)
                {
                    _missileContainer = value;
                    AudioSource.PlayClipAtPoint(_changeMissileContainerAudioClip, transform.position);
                }
            }
        }

        /// <summary>
        /// 当前发射物数量
        /// </summary>
        public int currentMissileCount => missileContainer ? missileContainer.currentCount : 0;

        /// <summary>
        /// 发射物容器容量
        /// </summary>
        public int missileCapacity => missileContainer ? missileContainer._capacity : 0;

        /// <summary>
        /// 发热时间
        /// </summary>
        [Name("发热时间")]
        [Min(0)]
        public float _hotAddTime = 0.5f;

        /// <summary>
        /// 冷却计时
        /// </summary>
        private float coolDownTimer = 0;

        /// <summary>
        /// 更新：计算枪械发热后的冷却过程
        /// </summary>
        protected virtual void Update()
        {
            if (coolDownTimer >= 0)
            {
                coolDownTimer -= Time.deltaTime;
            }
        }

        /// <summary>
        /// 获取发射物
        /// </summary>
        /// <returns></returns>
        public Missile GetMissile()
        {
            // 未冷却
            if (coolDownTimer > 0) return null;

            var bullet = missileContainer.GetMissile();
            if (bullet)
            {
                coolDownTimer += _hotAddTime;
            }
            return bullet;
        }

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData)
        {
            switch (GetInCmd(interactData.cmdName))
            {
                case nameof(Shoot): return currentMissileCount > 0;
                case nameof(SwitchMissileContainer): return interactData.interactable.GetComponent<MissileContainer>();
            }
            return base.CanInteract(interactData);
        }

        /// <summary>
        /// 射击
        /// </summary>
        [InteractCmd]
        [Name("射击")]
        public bool Shoot() => TryInteract(nameof(Shoot));

        /// <summary>
        /// 射击
        /// </summary>
        [InteractCmdFun(nameof(Shoot))]
        public EInteractResult Shoot(RayInteractData rayInteractData)
        {
            var missile = GetMissile();
            if (missile)
            {
                if (TryGetRay(rayInteractData.ray, out var outRay))
                {
                    missile.transform.position = outRay.origin;
                    missile.transform.forward = outRay.direction;
                    missile.OnFire(power);

                    if (_shootClip)
                    {
                        AudioSource.PlayClipAtPoint(_shootClip, transform.position, _shootVolume);
                    }
                    return EInteractResult.Success;
                }
            }
            return EInteractResult.Fail;
        }

        /// <summary>
        /// 切换弹夹
        /// </summary>
        [InteractCmd]
        [Name("切换弹夹")]
        public void SwitchMissileContainer() { }

        private bool TryGetRay(Ray? inRay, out Ray outRay)
        {
            outRay = default;

            // 优先使用传入射线
            if (inRay != null && inRay.HasValue)
            {
                outRay = inRay.Value;
                return true;
            }

            return _shootRay.TryGetRay(out outRay);
        }

    }
}
