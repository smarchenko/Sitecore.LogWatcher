<%@ Page Language="c#" Inherits="Sitecore.Web.UI.LogMessageViewer" CodePage="65001" %>

<%@ OutputCache Location="None" VaryByParam="none" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html lang="en" xml:lang="en" xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>Welcome to Log Message Viewer</title>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta name="CODE_LANGUAGE" content="C#" />
    <meta name="vs_defaultClientScript" content="JavaScript" />
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />

    <link href="/sitecore/shell/themes/standard/default/WebFramework.css" rel="Stylesheet" />
    <link href="/sitecore/admin/Wizard/UpdateInstallationWizard.css" rel="Stylesheet" />
    <script type="text/javascript" src="/sitecore/shell/Controls/lib/jQuery/jquery.js"></script>
    <script type="text/javascript" src="/sitecore/shell/controls/webframework/webframework.js"></script>
    
    <style type="text/css">
      
      .table-wrapper { height: 400px; margin-bottom:1em; overflow: auto; overflow-y: auto; margin-top: 0.5em; clear: both; }
      .table-wrapper table th { white-space:nowrap; text-align: left; border-bottom: solid 1px #ccc; padding-right: 8px; }
      .table-wrapper table td { vertical-align: top; padding: 4px 12px 4px 0;}
      .table-wrapper table td.message { white-space: nowrap; width:80px;}
      .table-wrapper table td.action { width:80px;}  
      .table-wrapper table td.database { width:60px;}    
      .table-wrapper tr.Row, .table-wrapper tr.AlternativeRow { cursor: pointer; }
      
      .filters { margin-top: 2em; overflow: auto;}
      #search { float: right; margin-top: 0.5em}  
      #search label { padding-right: 4px; }
      #search input { width: 350px; }
      
      .table-wrapper table tr.Row:hover, .table-wrapper table tr.AlternativeRow:hover{ background: #e8f5fd; }

      .table-wrapper td.message { padding-left: 24px; }
     
      .table-wrapper tr.Row:hover td.error, .table-wrapper tr.AlternativeRow:hover td.error { background: #e8f5fd url(/sitecore/admin/wizard/images/bullet_square_red.png) no-repeat 0 0; }
      .table-wrapper td.error { background: Transparent url(/sitecore/admin/wizard/images/bullet_square_red.png) no-repeat 0 0; }
      .table-wrapper tr.Row:hover td.warning, .table-wrapper tr.AlternativeRow:hover td.warning { background: #e8f5fd url(/sitecore/admin/wizard/images/bullet_square_yellow.png) no-repeat 0 0;  }
      .table-wrapper td.warning { background: Transparent url(/sitecore/admin/wizard/images/bullet_square_yellow.png) no-repeat 0 0;  }
      .table-wrapper tr.Row:hover td.collision, .table-wrapper tr.AlternativeRow:hover td.collision { background: #e8f5fd url(/sitecore/admin/wizard/images/bullet_square_blue.png) no-repeat 0 0;  }
      .table-wrapper td.collision { background: Transparent url(/sitecore/admin/wizard/images/bullet_square_blue.png) no-repeat 0 0;  }
      .table-wrapper tr.Row:hover td.info, .table-wrapper tr.AlternativeRow:hover td.info { background: #e8f5fd url(/sitecore/admin/wizard/images/bullet_square_grey.png) no-repeat 0 0;  }
      .table-wrapper td.info { background: Transparent url(/sitecore/admin/wizard/images/bullet_square_grey.png) no-repeat 0 0;  }
      
      .table-wrapper table td.description span.short-description:hover{ text-decoration: underline;}
    </style>

<script language="javascript" type="text/javascript">

    var showHideDescription = function (id) {
        $("#" + id).parent().toggle();
        if ($("#" + id).parent().is(":hidden")) {
            $("#" + id).parent().prev().attr("title", "Click to show more details");
        }
        else {
            $("#" + id).parent().prev().attr("title", "Click to hide details");
        }
    }


    var showMessages = function () {
        $(".messages-list").slideDown("slow");
        $(".wf-more a img").attr('src', '/sitecore/shell/Themes/Standard/Images/Progress/more_expanded.png');
        $(".wf-more a span").text('Hide installation messages');
    }

    var hideMessages = function () {
        $(".messages-list").slideUp();
        $(".wf-more a img").attr('src', '/sitecore/shell/Themes/Standard/Images/Progress/more_collapsed.png');
        $(".wf-more a span").text('Show installation messages');
    }

    var showHideMessages = function () {
        if ($(".messages-list").is(":hidden")) {
            showMessages();
        }
        else {
            hideMessageTypes();
            hideMessages();
        }
    }

    var showHideMessageTypes = function () {
        if ($("#MessageGroupsPanel").is(":hidden")) {
            showMessageTypes();
        }
        else {
            hideMessageTypes();
        }
    }

    var showMessageTypes = function () {
        if ($("#MessageGroupsPanel").is(":hidden")) {
            $("#MessageGroupsPanel").show();
        }
    }

    var hideMessageTypes = function () {
        if (!$("#MessageGroupsPanel").is(":hidden")) {
            $("#MessageGroupsPanel").hide();
        }
    }


    $(document).ready(function () {
        $(".wf-more a").click(showHideMessages);
        $("body").attr("class", "wf-layout-wide");

        $(document).click(function (e) {
            if (!$("#MessageGroupsPanel").is(":hidden")) {
                var element = $(e.target);
                if (!element.is("#MessageGroupsPanel *") && !element.is(".MessageTypesFilter *")) {
                    hideMessageTypes();
                }
            }
        });
    })


    function SetSelection(target) {
        var rng, sel;
        if (document.createRange) {
            rng = document.createRange();
            rng.selectNode(target)
            sel = window.getSelection();
            sel.removeAllRanges();
            sel.addRange(rng);
        } else {
            var rng = document.body.createTextRange();
            rng.moveToElementText(target);
            rng.select();
        }
    }
</script>

</head>
<body>
    <form id="mainform" method="post" runat="server" class="wf-container">
        <asp:ScriptManager ID="MANAGER" runat="server">
        </asp:ScriptManager>

        <div class="wf-content">
            <div class="table-wrapper">
                <asp:UpdatePanel ID="ResultGridUpdatePanel" runat="server">
                    <ContentTemplate>
                        <asp:GridView ID="ResultGrid" AllowSorting="True" BorderWidth="0px" runat="server"
                            AutoGenerateColumns="False" AllowPaging="True" Width="98%" OnRowDataBound="ResultGrid_RowDataBound"
                            OnRowCreated="ResultGrid_RowCreated" OnSorted="ResultGrid_Sorted" OnSorting="ResultGrid_Sorting"
                            GridLines="None" ShowFooter="true" PageSize="50" EmptyDataRowStyle-CssClass="EmptyGridMessage">
                            <Columns>
                                <asp:TemplateField HeaderText="Message Type" SortExpression="Level">
                                    <ItemTemplate>
                                        <asp:Label ID="MessageType" runat="server" Text='<%# Bind("Level") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Time" SortExpression="Time">
                                    <ItemTemplate>
                                        <asp:Label ID="TimeLbl" runat="server" Text='<%# Bind("Time") %>'></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle CssClass="database" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Description" SortExpression="Message">
                                    <ItemTemplate>
                                        <asp:Label ID="ShortDescription" CssClass="short-description" ToolTip="Click to show more details" runat="server"></asp:Label><div style="display: none;">
                                            <p style="padding: 0; margin: 0;">&nbsp</p>
                                            <asp:Label ID="Description" CssClass="long-description" runat="server"></asp:Label>
                                        </div>
                                    </ItemTemplate>
                                    <ItemStyle CssClass="description" />
                                </asp:TemplateField>
                            </Columns>
                            <AlternatingRowStyle CssClass="AlternativeRow" />
                            <HeaderStyle CssClass="HeaderBorder" />
                            <PagerSettings Mode="NumericFirstLast" Position="TopAndBottom" />
                            <PagerStyle CssClass="Paging" />
                            <RowStyle CssClass="Row" />
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
        <div class="wf-footer">
            <asp:Button ID="btnBack" CssClass="wf-backbutton" Text="Refresh" runat="server" />
        </div>
    </form>
</body>
</html>
