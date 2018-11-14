using Apps.Models.Enum;
using Apps.Models.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Apps.BLL.Flow
{
    public class FlowHelper
    {

        /// <summary>
        /// 获取指定类型的HTML表单
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="id"></param>
        /// <param name="attrNo"></param>
        /// <param name="values">可能是下拉单选复选的默认值,隔开</param>
        /// <param name="isCreate">是否是创建的，如果是需要显示复选，单选的额外</param>
        /// <returns></returns>
        public string GetInput(string type, string id, string attrNo,string values,bool isCreate)
        {
            string str = "";
            if (type == "文本")
            {
                str = "<input id='" + id + "' class='input'  name='" + attrNo + "' value='" + values + "' type='text' />";
            }
            else if (type == "多行文本")
            {
                str = "<textarea id='" + id + "' class='input' name='" + attrNo + "'  >" + values + "</textarea>";
            }
            else if (type == "日期")
            {
                str = "<input type='text' class='input' name='" + attrNo + "'  id='" + id + "'  class='Wdate' onfocus=\"WdatePicker({dateFmt:'yyyy-MM-dd'})\"  />";
            }
            else if (type == "时间")
            {
                str = "<input type='text' class='input' name='" + attrNo + "'  id='" + id + "' value='" + values + "'  class='Wdate' onfocus=\"WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'})\"   />";
            }
            else if (type == "数字")
            {
                str = "<input type='number' class='input' name='" + attrNo + "'  id='" + id + "' value='" + values + "' />";
            }
            else if (type == "附件")
            {
                str = "<input style='width:60%;float:left' type='text' name='" + attrNo + "'  id='" + id + "' maxlength='255' class='input uploadinput'><a onclick=\"$('#" + attrNo + "FileUpload').trigger('click')\" class='files'>浏览</a><input class=\'displaynone\' type=\'file\' id=\'" + attrNo + "FileUpload\' name=\'" + attrNo + "FileUpload\' onchange=\"Upload(\'SingleFile\', \'" + id + "\', \'" + attrNo + "FileUpload\');\"><span class=\'uploading\'>上传中</span>";
            }
            else if (type == "数字")
            {
                str = "<input type='number' class='input' name='" + attrNo + "'  id='" + id + "' value='"+ values + "' />";
            }
            else if (type == "下拉框")
            {
                string options = "";
                if (!string.IsNullOrEmpty(values))
                {
                    //分解默认值
                    string[] opts = values.Split(',');
                    foreach(var r in opts)
                        options = options + "<option value ='"+r+ "'>" + r + "</option>";
                }
                str = "<select name='" + attrNo + "' id='" + id + "'>"+ options + "</select>";
            }
            else if (type == "单选按钮")
            {
                string options = "";
                if (!string.IsNullOrEmpty(values))
                {
                    //分解默认值
                    string[] opts = values.Split(',');
                    foreach (var r in opts)
                        options = options + "<input type='radio' name='" + attrNo + "' id='" + id + "' value='"+r+ "' />" + (isCreate ? r :"");
                }
                str = options;
            }
            else if (type == "复选框")
            {
                string options = "";
                if (!string.IsNullOrEmpty(values))
                {
                    //分解默认值
                    string[] opts = values.Split(',');
                    foreach (var r in opts)
                        options = options + "<input type='checkbox' name='" + attrNo + "' id='" + id + "' value='" + r + "' />"+ (isCreate?r:"");
                }
                str = options;
            }
            else if (type == "人员弹出框")
            {
                str = @"
                      
                        <input type='hidden' name='"+ attrNo + @"' id='"+ id + @"'/>
                        <span name='SelLookUp'>
                        <input id='" + id + @"List' name='" + attrNo + @"List' readonly='readonly' type='text' style='width: 90px; display: inline; background: #dedede; '>
                        <a class='fa fa-plus-square color-gray fa-lg' id='selExc" + id + @"' href='javascript:void(0)'></a></span>
                        <script type = 'text/javascript' >
                            $(function()
                            {
                                $('#selExc" + id + @"').click(function() {
                                    $('#modalwindow').html(""<iframe width='100%' height='100%' scrolling='no' frameborder='0'' src='/SysHelper/UserLookUp?key=" + id+@"&val="+id+ @"List'></iframe>"");
                                    $('#modalwindow').window({ title: '选择人员', width: 620, height: 388, iconCls: 'fa fa-plus' }).window('open');
                                });
                            });

                            function SetSelResult(result, resultName,key,val) {
                            $('#'+key).val(result);
                            $('#' + val).val(resultName);
                            }
                            function GetSelResult(key, val) {
                                var arrayObj = new Array()
                                arrayObj[0] = $('#' + key).val();
                                arrayObj[1] = $('#' + val).val();
                                return arrayObj;
                            }
                            //ifram 返回
                            function frameReturnByClose()
                            {
                                $('#modalwindow').window('close');
                            }
                        </script>";
            }
            return str;
        }
        //对比条件
        public bool Judge(string attrType, string rVal, string cVal, string lVal)
        {
            if (attrType == "数字")
            {
                double rVald = Convert.ToDouble(rVal);
                double lVald = Convert.ToDouble(lVal);
                if (cVal == "==")
                {
                    if (rVald == lVald)//为真
                    {
                        return false;
                    }
                }
                if (cVal == ">")
                {
                    if (rVald > lVald)//为真
                    {
                        return false;
                    }
                }
                if (cVal == "<")
                {
                    if (rVald < lVald)//为真
                    {
                        return false;
                    }
                }
                if (cVal == ">=")
                {
                    if (rVald >= lVald)//为真
                    {
                        return false;
                    }
                }
                if (cVal == "<=")
                {
                    if (rVald <= lVald)//为真
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        //获取对应的提交的值
        public string GetFormAttrVal(string attrId, Flow_FormModel formModel, Flow_FormContentModel formContentModel)
        {
            //获得对象的类型，model
            Type formContentType = formContentModel.GetType();
            Type formType = formModel.GetType();
           
            //查找名称为"A-Z"的属性
            string[] arrStr = { "AttrA", "AttrB", "AttrC", "AttrD", "AttrE", "AttrF", "AttrG", "AttrH", "AttrI", "AttrJ", "AttrK"
                                  , "AttrL", "AttrM", "AttrN", "AttrO", "AttrP", "AttrQ", "AttrR", "AttrS", "AttrT", "AttrU"
                                  , "AttrV", "AttrW", "AttrX", "AttrY", "AttrZ"};
            foreach (string str in arrStr)
            {
               
                object o = formType.GetProperty(str).GetValue(formModel, null);
                object v = formContentType.GetProperty(str).GetValue(formContentModel, null);
                if (o != null)
                {
                    //查找model类的Class对象的"str"属性的值
                    if (o.ToString() == attrId) {
                        return v.ToString();
                    }
                }
            }
            return "";
        }

        public string GetCurrentStepCheckIdByStepCheckModelList(List<Flow_FormContentStepCheckModel> stepCheckModelList)
        {
            string stepCheckId = "";
            for (int i = stepCheckModelList.Count() - 1; i >= 0; i--)
            {
                //获得在进行中的单子
                if (stepCheckModelList[i].State == (int)FlowStateEnum.Progress)// || stepCheckModelList[i].State == (int)FlowStateEnum.Reject
                {
                    stepCheckId = stepCheckModelList[i].Id;
                    if (i != 0)//查看上一个审核状态
                    {
                        if (stepCheckModelList[i - 1].State != 1)//查看上一步是否没有审核完成或是不通过
                        {
                            stepCheckId = "";//等于空，终止于上一环节
                        }
                    }
                }
            }
            return stepCheckId;
        }
    }
}