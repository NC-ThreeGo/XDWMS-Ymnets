function base64(content) {
    return window.btoa(unescape(encodeURIComponent(content)));
}
function exportOffice(dom, tableID, fName) {
    var type = 'excel';
    var table = document.getElementById(tableID);
    var excelContent = table.innerHTML;
    var excelFile = "<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:x='urn:schemas-microsoft-com:office:" + type + "' xmlns='http://www.w3.org/TR/REC-html40'>";
    excelFile += "<head>";
    excelFile += "<meta http-equiv=Content-Type; content=text/html;charset=UTF-8>";
    excelFile += "<!--[if gte mso 9]>";
    excelFile += "<xml>";
    excelFile += "<x:ExcelWorkbook>";
    excelFile += "<x:ExcelWorksheets>";
    excelFile += "<x:ExcelWorksheet>";
    excelFile += "<x:Name>";
    excelFile += "{worksheet}";
    excelFile += "</x:Name>";
    excelFile += "<x:WorksheetOptions>";
    excelFile += "<x:DisplayGridlines/>";
    excelFile += "</x:WorksheetOptions>";
    excelFile += "</x:ExcelWorksheet>";
    excelFile += "</x:ExcelWorksheets>";
    excelFile += "</x:ExcelWorkbook>";
    excelFile += "</xml>";
    excelFile += "<![endif]-->";
    excelFile += "</head>";
    excelFile += "<body><table>";
    excelFile += excelContent;
    excelFile += "</table></body>";
    excelFile += "</html>";
    var base64data = "base64," + base64(excelFile);
    switch (type) {
        case 'excel':
            dom.href = 'data:application/vnd.ms-' + type + ';' + base64data;;//必须是a标签，否则无法下载改名
            dom.download = fName;
            break;
    }
}