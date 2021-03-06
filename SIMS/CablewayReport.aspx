﻿<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SIMSSafety.Master" CodeBehind="CablewayReport.aspx.vb" Inherits="SIMS.CablewayReport" %>
<%@ MasterType  virtualPath="~/SIMSSafety.master" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
        .InspOverdue
        {
            font-weight:bold;
            color: red;
        }
        .InspWithin30days
        {
            color:red;
        }
        .InspWithin6mo
        {
            color:orange;
        }
        * + html .riSingle .riTextBox
        {
            width: 10px !important;
            height: 14px !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" Runat="Server">
    <telerik:RadAjaxManager ID="ram" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rgInspections">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgInspections" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rgStatus">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgStatus" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Web20">
    </telerik:RadAjaxLoadingPanel>
    <asp:Panel ID="pnlNW" runat="server">
        <div style="height:60px;width:100%;">
            <div style="float:right;">
                <asp:ImageMap ID="imInstructions" runat="server" ImageUrl="images/manualandhelplinks.png" BorderStyle="None">
                    <asp:RectangleHotSpot AlternateText="Manned Cableway Inspection e-Form" HotSpotMode="Navigate" 
                        NavigateUrl="https://sims.water.usgs.gov/SIMSShare/MannedCablewayInspectionE-form.pdf" Bottom="33" Left="38" Right="210" Top="15" Target="_blank" />
                    <asp:RectangleHotSpot AlternateText="Bank-Operated Cableways Inspection Checklist" HotSpotMode="Navigate" 
                        NavigateUrl="https://sims.water.usgs.gov/SIMSShare/Appendix41-3-SRHedited.docx" Bottom="58" Left="38" Right="210" Top="38" Target="_blank" />
                    <asp:RectangleHotSpot AlternateText="USGS Manual" HotSpotMode="Navigate" 
                        NavigateUrl="http://www.usgs.gov/usgs-manual/handbook/hb/445-2-h/ch41.html" Bottom="33" Left="270" 
                        Right="470" Target="_blank" Top="10" />
                    <asp:RectangleHotSpot AlternateText="WMA Memo 13.03" HotSpotMode="Navigate"
                        NavigateUrl="https://sims.water.usgs.gov/SIMSShare/WMAMemorandum13.03.pdf" Bottom="60" Left="270" Right="460" Target="_blank" Top="38" />
                    <asp:RectangleHotSpot AlternateText="CMI Instructions (.pptx)" HotSpotMode="Navigate" 
                        NavigateUrl="https://sims.water.usgs.gov/SIMSShare/SIMSCMIInstructions.pptx" Left="520" 
                        Right="800" Target="_blank" Top="5" Bottom="60" />
                </asp:ImageMap>
            </div>
        </div>
        <telerik:RadGrid ID="rgInspections" runat="server" AutoGenerateColumns="false" EnableLinqExpressions="false" 
            Skin="Web20" GridLines="None" ShowStatusBar="true" PageSize="50" OnNeedDataSource="rgInspections_NeedDataSource"
            AllowSorting="true" 
            AllowMultiRowSelection="false" 
            AllowFiltering="true"
            AllowPaging="true"
            AllowAutomaticDeletes="true" 
            OnItemDataBound="rgInspections_ItemDataBound"
            OnPreRender="rgInspections_PreRender">
            <PagerStyle Mode="NextPrevNumericAndAdvanced" />
            <MasterTableView DataKeyNames="site_no" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" AllowFilteringByColumn="true">
                <Columns>
                    <telerik:GridBoundColumn DataField="region_cd" UniqueName="region_cd" FilterControlWidth="20px" HeaderText="Region" HeaderStyle-Width="35px" />
                    <telerik:GridBoundColumn DataField="wsc_cd" UniqueName="wsc_cd" FilterControlWidth="20px" HeaderText="WSC" HeaderStyle-Width="35px"  />
                    <telerik:GridBoundColumn DataField="site_no" UniqueName="site_no" Display="false" />
                    <telerik:GridTemplateColumn DataField="site_no_nm" SortExpression="site_no" UniqueName="site_no_nm" HeaderText="Site" FilterControlWidth="150px" HeaderStyle-Width="400px">
                        <ItemTemplate>
                            <asp:HyperLink ID="hlSite" runat="server" Text='<%# Bind("site_no_nm") %>' Target="_blank" />&nbsp;
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="last_inspection_dt" UniqueName="last_inspection_dt" HeaderText="Last Insp Date" DataFormatString="{0:MM/dd/yyyy}" AllowFiltering="false" HeaderStyle-Width="70px" />
                    <telerik:GridBoundColumn DataField="last_visit_dt" HeaderText="Last Visit Date" UniqueName="last_visit_dt" DataFormatString="{0:MM/dd/yyyy}" AllowFiltering="false" HeaderStyle-Width="70px" />
                    <telerik:GridBoundColumn DataField="cableway_inspection_freq" UniqueName="cableway_inspection_freq" HeaderText="Insp Freq" FilterControlWidth="20px" HeaderStyle-Width="35px"  />
                    <telerik:GridBoundColumn DataField="status" HeaderText="Status"  UniqueName="status" FilterControlWidth="50px" HeaderStyle-Width="200px" />
                    <telerik:GridBoundColumn DataField="cableway_type_cd" HeaderText="Type" UniqueName="type" FilterControlWidth="40px" HeaderStyle-Width="50px"  />
                    <telerik:GridBoundColumn DataField="aerial_marker_req" HeaderText="Aerial Marker Required" UniqueName="aerial_marker_req" FilterControlWidth="40px" HeaderStyle-Width="50px" />
                    <telerik:GridBoundColumn DataField="aerial_marker_inst" HeaderText="Aerial Marker Installed" UniqueName="aerial_marker_inst" FilterControlWidth="40px" HeaderStyle-Width="50px" />
                    <telerik:GridBoundColumn DataField="wsc_id" UniqueName="wsc_id" Display="false" />
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </asp:Panel>
    <asp:Panel ID="pnlStatus" runat="server">
        <p>The Cableway Inspection Status Report shows all <strong>active cableways</strong>, their last inspection date, and their next inspection date.  
            Currently, the next inspection date is <strong>based on a one year frequency</strong> no matter the inspection frequency entered in the Cableway Management Interface. Next inspection dates 
            in <span class="InspOverdue">bold red</span> are overdue, dates in <span class="InspWithin30days">normal red</span> are due within 30 days, and dates in <span class="InspWithin6mo">normal orange</span> are due within
            six months.  Use the filters below the table headings to filter by Region, WSC, or Office.  Click on the table headings to sort by that column. 
        </p>
        <telerik:RadGrid ID="rgStatus" runat="server" AutoGenerateColumns="false" EnableLinqExpressions="false" 
            Skin="Web20" GridLines="None" ShowStatusBar="true" PageSize="50" OnNeedDataSource="rgStatus_NeedDataSource"
            AllowSorting="true" 
            AllowMultiRowSelection="false" 
            AllowFiltering="true"
            AllowPaging="true"
            AllowAutomaticDeletes="true" 
            OnItemDataBound="rgStatus_ItemDataBound"
            OnPreRender="rgStatus_PreRender">
            <PagerStyle Mode="NextPrevNumericAndAdvanced"  />
            <MasterTableView DataKeyNames="site_no" AllowMultiColumnSorting="true" Width="100%" CommandItemDisplay="None" AllowFilteringByColumn="true">
                <Columns>
                    <telerik:GridBoundColumn DataField="site_no" UniqueName="site_no" Display="false" />
                    <telerik:GridBoundColumn DataField="region_cd" UniqueName="region_cd" FilterControlWidth="25px" HeaderText="Region" HeaderStyle-Width="35px" />
                    <telerik:GridBoundColumn DataField="wsc_cd" UniqueName="wsc_cd" FilterControlWidth="25px" HeaderText="WSC" HeaderStyle-Width="35px"  />
                    <telerik:GridBoundColumn DataField="office_cd" UniqueName="office_cd" HeaderText="Office Code" FilterControlWidth="25px" HeaderStyle-Width="35px" />
                    <telerik:GridTemplateColumn DataField="site_no_nm" SortExpression="site_no" UniqueName="site_no_nm" HeaderText="Site" FilterControlWidth="150px" HeaderStyle-Width="400px">
                        <ItemTemplate>
                            <asp:HyperLink ID="hlSite" runat="server" Text='<%# Bind("site_no_nm") %>' Target="_blank" />&nbsp;
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="last_inspection_dt" UniqueName="last_inspection_dt" HeaderText="Last Insp Date" DataFormatString="{0:MM/dd/yyyy}" AllowFiltering="false" HeaderStyle-Width="80px" />
                    <telerik:GridBoundColumn DataField="next_inspection_dt" HeaderText="Next Insp Date" UniqueName="next_inspection_dt" DataFormatString="{0:MM/dd/yyyy}" AllowFiltering="false" HeaderStyle-Width="80px"  />
                    <telerik:GridBoundColumn DataField="days_to_next" HeaderText="Days to Next"  UniqueName="days_to_next" FilterControlWidth="35px" HeaderStyle-Width="55px" />
                    <telerik:GridBoundColumn DataField="status" HeaderText="Status"  UniqueName="status" FilterControlWidth="50px" HeaderStyle-Width="220px" />
                    <telerik:GridBoundColumn DataField="wsc_id" UniqueName="wsc_id" Display="false" />
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </asp:Panel>
</asp:Content>
