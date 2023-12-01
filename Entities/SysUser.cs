using FreeSql.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebAOT.Entities
{
    /// <summary>
    /// 系统用户表
    /// </summary>
    [Table(Name = "net_sysuser")]
    public class SysUser 
    {
       
        [Column(IsPrimary = true)]
        public long id { get; set; }

        /// <summary>
        /// 账号
        /// </summary>

     
        public virtual string Account { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
       
      
        [System.Text.Json.Serialization.JsonIgnore]
      
        public virtual string Password { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
       
        
        public virtual string RealName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
      
      
        public string? NickName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
       
      
        public string? Avatar { get; set; }

       

        /// <summary>
        /// 年龄
        /// </summary>
       
        public int Age { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
       
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
       
       
        public string? Nation { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
      
        
        public string? Phone { get; set; }

     
        /// <summary>
        /// 身份证号
        /// </summary>
       
       
        public string? IdCardNum { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
      
        
        public string? Email { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
      
        
        public string? Address { get; set; }


        /// <summary>
        /// 政治面貌
        /// </summary>
       
        
        public string? PoliticalOutlook { get; set; }

        /// <summary>
        /// 毕业院校
        /// </summary>COLLEGE
       
       
        public string? College { get; set; }

        /// <summary>
        /// 办公电话
        /// </summary>
       
        
        public string? OfficePhone { get; set; }

        /// <summary>
        /// 紧急联系人
        /// </summary>
       
        
        public string? EmergencyContact { get; set; }

        /// <summary>
        /// 紧急联系人电话
        /// </summary>
       
        
        public string? EmergencyPhone { get; set; }

        /// <summary>
        /// 紧急联系人地址
        /// </summary>
      
        
        public string? EmergencyAddress { get; set; }

        /// <summary>
        /// 个人简介
        /// </summary>
       
        
        public string? Introduction { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
      
        public int OrderNo { get; set; } = 100;

      

        /// <summary>
        /// 备注
        /// </summary>
      
       
        public string? Remark { get; set; }


        /// <summary>
        /// 机构Id
        /// </summary>
       
        public long OrgId { get; set; }

      

        /// <summary>
        /// 职位Id
        /// </summary>
      
        public long PosId { get; set; }

      

        /// <summary>
        /// 工号
        /// </summary>
      
        
        public string? JobNum { get; set; }

        /// <summary>
        /// 职级
        /// </summary>
       
       
        public string? PosLevel { get; set; }

        /// <summary>
        /// 入职日期
        /// </summary>
      
        public DateTime? JoinDate { get; set; }

        /// <summary>
        /// 最新登录Ip
        /// </summary>
      
       
        public string? LastLoginIp { get; set; }

        /// <summary>
        /// 最新登录地点
        /// </summary>
      
       
        public string? LastLoginAddress { get; set; }

        /// <summary>
        /// 最新登录时间
        /// </summary>
      
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 最新登录设备
        /// </summary>
       
       
        public string? LastLoginDevice { get; set; }

        /// <summary>
        /// 电子签名
        /// </summary>
       
        
        public string? Signature { get; set; }
    }
}
