using System;
using System.ComponentModel;
using NetSql.Core.Entities;

namespace SampleConsole.Domain.Account
{
    /// <summary>
    /// 账户
    /// </summary>
    public class Account : Entity
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime LastLoginTime { get; set; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
        public string LastLoginIP { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public AccountStatus Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建人
        /// </summary>
        public Guid Creator { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastModifiedTime { get; set; }

        /// <summary>
        /// 最后修改人
        /// </summary>
        public Guid LastModifiedBy { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 注销时间
        /// </summary>
        public DateTime ClosedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 注销人
        /// </summary>
        public Guid ClosedBy { get; set; } = Guid.Empty;
    }

    /// <summary>
    /// 账户状态
    /// </summary>
    public enum AccountStatus
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        UnKnown = -1,
        /// <summary>
        /// 未激活
        /// </summary>
        [Description("未激活")]
        Inactive,
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Enabled,
        /// <summary>
        /// 禁用
        /// </summary>
        [Description("禁用")]
        Disabled,
        /// <summary>
        /// 注销
        /// </summary>
        [Description("注销")]
        Closed
    }
}
