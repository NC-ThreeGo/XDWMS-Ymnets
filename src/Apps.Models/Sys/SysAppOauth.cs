using System;

namespace Apps.Model.Sys
{
    /// <summary>
    /// 开放平台:实体类
    /// </summary>
    [Serializable]
    public partial class SysAppOauth
    {
        public SysAppOauth()
        { }
        #region Model
        private int _id;
        private string _title = "";
        private string _img_url = "";
        private string _app_id;
        private string _app_key;
        private string _remark = "";
        private int _sort_id = 99;
        private int _is_lock = 0;
        private string _api_path = "";
        /// <summary>
        /// 自增ID
        /// </summary>
        public int id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string title
        {
            set { _title = value; }
            get { return _title; }
        }
        /// <summary>
        /// 显示图片
        /// </summary>
        public string img_url
        {
            set { _img_url = value; }
            get { return _img_url; }
        }
        /// <summary>
        /// AppId
        /// </summary>
        public string app_id
        {
            set { _app_id = value; }
            get { return _app_id; }
        }
        /// <summary>
        /// AppKey
        /// </summary>
        public string app_key
        {
            set { _app_key = value; }
            get { return _app_key; }
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string remark
        {
            set { _remark = value; }
            get { return _remark; }
        }
        /// <summary>
        /// 排序数字
        /// </summary>
        public int sort_id
        {
            set { _sort_id = value; }
            get { return _sort_id; }
        }
        /// <summary>
        /// 是否启用
        /// </summary>
        public int is_lock
        {
            set { _is_lock = value; }
            get { return _is_lock; }
        }
        /// <summary>
        /// 接口目录
        /// </summary>
        public string api_path
        {
            set { _api_path = value; }
            get { return _api_path; }
        }
        #endregion Model

    }
}