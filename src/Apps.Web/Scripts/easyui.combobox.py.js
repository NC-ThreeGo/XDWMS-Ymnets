document.write("<script language=javascript src='/Scripts/ChinesePY.js'></script >");

//搜索全拼或简拼或汉字
function filterComboPY(q, row)
{
    if (/^[a-zA-Z]*$/.test(q))
    {
        var opts = $(this).combobox('options');
        var tmpValue = row.value;
        var tmpText = row.text;
        //大写转换,默认使用小写查询
        var q = q.toLowerCase();
        //默认小于等于3个拼音为简写
        //if (q.length <= 3)
        {
            row.value = Pinyin.GetJP(row.value);
            row.text = Pinyin.GetJP(row.text);
            if (row[opts.textField].indexOf(q) == 0)
            {
                row.value = tmpValue;
                row.text = tmpText;
                return true;
            } else
            {
                row.value = tmpValue;
                row.text = tmpText;
                return false
            }
        }
        //默认大于3的为全拼
        //if (q.length > 3)
        //{
        //    row.value = Pinyin.GetQP(row.value);
        //    row.text = Pinyin.GetQP(row.text);
        //    if (row[opts.textField].indexOf(q) == 0)
        //    {
        //        row.value = tmpValue;
        //        row.text = tmpText;
        //        return true;
        //    } else
        //    {
        //        row.value = tmpValue;
        //        row.text = tmpText;
        //        return false
        //    }
        //}
    } else if (/^[\u4e00-\u9fa5]*$/.test(q))
    {
        var opts = $(this).combobox('options');
        return row[opts.textField].indexOf(q) == 0;
    }
}

