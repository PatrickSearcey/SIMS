<%@ Control Language="C#" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI.Editor" TagPrefix="tools" %>
<script type="text/javascript">
	Type.registerNamespace("Telerik.Web.UI.Widgets");

	Telerik.Web.UI.Widgets.TableProperties = function (element) {
		Telerik.Web.UI.Widgets.TableProperties.initializeBase(this, [element]);

		this._clientParameters = null;
		this._currentRowSpan = 1;
		this._currentColSpan = 1;
		this._spinBoxColumns = null;
		this._spinBoxColumnSpan = null;
		this._spinBoxRows = null;
		this._spinBoxRowSpan = null;
		this._selectedCell = null;
		this._selectedCellIndex = -1;
		this._victimColumns = new Array();
		this._victimRows = new Array();
		this._rowsCount = 0;
		this._tableToModify = null;
		this._tablePreviewControl = null;
		this._tableWizardDialog = null;
		this._tableLayoutBuilderEngine = new Telerik.Web.UI.LayoutBuilderEngine();
	}

	Telerik.Web.UI.Widgets.TableProperties.prototype = {
		initialize: function () {
			Telerik.Web.UI.Widgets.TableProperties.callBaseMethod(this, "initialize");
			this._initializeChildren();

			var unitsData = this._heightUnit.get_itemData();
			var units = [];
			for (var i = 0; i < unitsData.length; i++) {
				units.push(unitsData[i].value);
			}
			this._reUnits = new RegExp(units.join("|") + "$", "i");
		},

		_initializeChildren: function () {
			this._spinBoxDelegate = Function.createDelegate(this, this._spinBoxValueChanged);
			this._selectedCellDelegate = Function.createDelegate(this, this._onSelectedCellChanged);
			this._styleBuilderBtnClickDelegate = Function.createDelegate(this, this._styleBuilderClicked);

			this._tablePreviewControl = new Telerik.Web.UI.Widgets.TablePreview();
			this._tablePreviewControl.set_previewHolder($get("TableProperties_PreviewTableHolder"));
			this._tablePreviewControl._onSelectedCellChanged = this._selectedCellDelegate;

			this._spinBoxColumns = this._initializeSpinBox("SpinBoxColumns");
			this._spinBoxColumnSpan = this._initializeSpinBox("SpinBoxColumnSpan");
			this._spinBoxRows = this._initializeSpinBox("SpinBoxRows");
			this._spinBoxRowSpan = this._initializeSpinBox("SpinBoxRowSpan");

			this._spinBoxHeight = $find("SpinBoxTableHeight");
			this._spinBoxWidth = $find("SpinBoxTableWidth");

			this._heightUnit = $find("HeightUnit");
			this._widthUnit = $find("WidthUnit");

			this._spinBoxCellSpacing = $find("SpinBoxTableCellSpacing");
			this._spinBoxCellPadding = $find("SpinBoxTableCellSPadding");

			this._alignmentSelectorTable = $find("TableAlignment");

			this._colorPicker = $find("ColorPicker1");
			this._classSelector = $find("TableCssClass");
			this._imageCaller = $find("TableBgImage");

			this._styleBuilder = $get("StyleBuilderBtnTP");

			this._styleBuilder.innerHTML = localization["MoreTableStyling"];
			$(this._styleBuilder).on("click", this._styleBuilderBtnClickDelegate);
		},

		clientInit: function (clientParameters) {
			this._clientParameters = clientParameters;
			var table = this._tableToModify = this._clientParameters.originalTable;
			this._tableLayoutBuilderEngine.set_selectedTable(this._tableToModify);

			this._selectedCell = this._clientParameters.selectedCell;

			if (this._colorPicker._popupVisible) this._colorPicker.hide();
			this._colorPicker.set_items(clientParameters.Colors);
			this._colorPicker.set_addCustomColorText(localization["AddCustomColor"]);

			this._classSelector.set_showText(true);
			this._classSelector.set_clearclasstext(localization["ClearClass"]);
			this._classSelector.set_text(localization["ApplyClass"]);

			this._classSelector.set_items(clientParameters.CssClasses);
			this._classSelector.add_valueSelected(this._cssValueSelected);

			if (table.className) {
				this._classSelector.updateValue(table.className);
			}

			this._editorRef = clientParameters.editor;
			this._imageCaller.set_editor(this._editorRef);

			var tableStyle = table.style;
			var tableWidth = tableStyle.width ? tableStyle.width : table.getAttribute("width");
			var tableHeight = tableStyle.height ? tableStyle.height : table.getAttribute("height");

			if (tableWidth && !this._reUnits.test(tableWidth)) {
				tableWidth = table.offsetWidth + "px";
			}

			if (tableHeight && !this._reUnits.test(tableHeight)) {
				tableHeight = table.offsetHeight + "px";
			}

			this._setSize(tableHeight, this._spinBoxHeight, this._heightUnit);
			this._setSize(tableWidth, this._spinBoxWidth, this._widthUnit);

			this._alignmentSelectorTable.setTagName("TABLE");
			this._alignmentSelectorTable.updateValue(table.align);

			this._loadData();
		},

		_setSize: function (size, sizeControl, unitControl) {
			size = size || "";
			var unitMatch = size.match(this._reUnits);
			var unitItem = unitControl.findItemByValue(unitMatch ? unitMatch[0].toLowerCase() : "px");

			unitItem.set_selected(true);
			sizeControl.set_value(size ? parseFloat(size) : size);
		},

		_initializeSpinBox: function (boxId) {
			var spinBox = $find(boxId);
			spinBox.add_valueSelected(this._spinBoxDelegate);
			spinBox.set_value(9999);
			return spinBox;
		},

		_spinBoxValueChanged: function (sender, args) {
			var oldValue = args.get_oldValue();
			var newValue = args.get_newValue();
			var diff = newValue - oldValue;
			//difference should be either +1 or -1
			if (diff != -1 && diff != 1) {
				return;
			}
			switch (sender) {
				case this._spinBoxColumns:
					return (diff == 1) ? this._addNewColumn() : this._deleteLastColumn();
					break;
				case this._spinBoxColumnSpan:
					return (diff == 1) ? this._increaseColSpan() : this._decreaseColSpan();
					break;
				case this._spinBoxRows:
					return (diff == 1) ? this._addNewRow() : this._deleteLastRow();
					break;
				case this._spinBoxRowSpan:
					return (diff == 1) ? this._increaseRowSpan() : this._decreaseRowSpan();
					break;
				default:
					break;
			}
		},

		_styleBuilderClicked: function (ev) {
			//Editor object is supplied to all dialogs in the dialog parameters
			var editor = this._clientParameters.editor;
			var callbackFunction = Function.createDelegate(this, function (sender, args) {
				//Returned link - TODO: Use args.get_value() when the dialog returm methods are changed to return proper args object
				var resulTable = args.get_value();

				var cssText = resulTable.style.cssText;
				this._cssText = cssText;
				this._tableToModify.style.cssText = cssText;
				this._loadData();
			});

			//NEW: Use EditorCommandEventArgs
			var parameterTable = this._tableToModify.cloneNode(true);
			this._update(parameterTable);
			var argument = new Telerik.Web.UI.EditorCommandEventArgs("StyleBuilder", null, parameterTable);

			Telerik.Web.UI.Editor.CommandList._getDialogArguments(argument, "*", editor, "StyleBuilder");
			argument.fontNames = editor.get_fontNames();
			editor.showDialog("StyleBuilder", argument, callbackFunction);

			ev.preventDefault();
		},

		_loadData: function () {
			//clear dialog data because the page is not reloaded each time the dialog is opened!
			this._currentRowSpan = 1;
			this._currentColSpan = 1;
			this._selectedCell = null;
			this._selectedCellIndex = -1;
			this._victimColumns = new Array();
			this._victimRows = new Array();

			//clear the table preview control as well		
			this._tablePreviewControl.deSelectAllCells();
			if (this._tableToModify) {
				this._rowsCount = this._tableToModify.rows.length;
			}
			this._tablePreviewControl._updateTable(this._tableToModify, this._selectedCell);
			this._checkButtonAvailability();
		},

		_onSelectedCellChanged: function () {
			this._synchronizeSelectedCell();
			this._checkButtonAvailability();
		},

		_synchronizeSelectedCell: function () {
			var previewSelectedCell = this._tablePreviewControl.get_selectedCell();
			var previewTable = this._tablePreviewControl.get_previewTable();

			var previewTableRows = previewTable.rows;
			this._selectedCell = null;
			this._selectedCellIndex = -1;
			for (var i = 0; i < previewTableRows.length; i++) {
				var previewTableCells = previewTableRows[i].cells;
				for (var j = 0; j < previewTableCells.length; j++) {
					if (previewTableCells[j] == previewSelectedCell) {
						this._selectedCell = this._tableToModify.rows[i].cells[j];
						this._tableLayoutBuilderEngine.set_selectedCell(this._selectedCell);
						this._selectedCellIndex = j;
						if (this._tableWizardDialog)
							this._tableWizardDialog._synchronizeSelection(this._selectedCell, this._selectedCellIndex);
						return;
					}
				}
			}
		},

		_checkButtonAvailability: function () {
			if (!this._selectedCell) {
				var table = this._tablePreviewControl.get_previewTable();

				//check row and column count for the row and column spinboxes
				if (this._getNamedNodesLength(table.firstChild, "TR") > 1)
					this._spinBoxRows.set_enabledDecrease(true);
				else
					this._spinBoxRows.set_enabledDecrease(false);

				if (this._getMaxColumns() > 1)
					this._spinBoxColumns.set_enabledDecrease(true);
				else
					this._spinBoxColumns.set_enabledDecrease(false);

				this._spinBoxColumnSpan.set_enabledDecrease(false);
				this._spinBoxRowSpan.set_enabledDecrease(false);
			}
			else {
				var layoutBulider = this._tableLayoutBuilderEngine;
				var layoutBuliderSelectedCell = layoutBulider._selectedCell;
				var spinBoxColumnSpan = this._spinBoxColumnSpan;
				var spinBoxRowSpan = this._spinBoxRowSpan;
				var spinBoxColumns = this._spinBoxColumns;
				var spinBoxRows = this._spinBoxRows;

				//_spinBoxColumnSpan
				if (layoutBulider.canMergeRight())
					spinBoxColumnSpan.set_enabledIncrease(true);
				else
					spinBoxColumnSpan.set_enabledIncrease(false);

				if (layoutBulider._getColSpan(layoutBuliderSelectedCell) > 1)
					spinBoxColumnSpan.set_enabledDecrease(true);
				else
					spinBoxColumnSpan.set_enabledDecrease(false);


				//_spinBoxColumnSpan
				if (layoutBulider.canMergeDown())
					spinBoxRowSpan.set_enabledIncrease(true);
				else
					spinBoxRowSpan.set_enabledIncrease(false);

				if (layoutBulider._getRowSpan(layoutBuliderSelectedCell) > 1)
					spinBoxRowSpan.set_enabledDecrease(true);
				else
					spinBoxRowSpan.set_enabledDecrease(false);

				//_spinBoxColumns   
				if (layoutBulider._tableStateColsCount != 1)
					spinBoxColumns.set_enabledDecrease(true);
				else
					spinBoxColumns.set_enabledDecrease(false);

				//_spinBoxRows
				if (layoutBulider._tableStateRowsCount != 1)
					spinBoxRows.set_enabledDecrease(true);
				else
					spinBoxRows.set_enabledDecrease(false);
			}
		},

		_getNamedNodesLength: function (node, name) {
			var counter = 0;
			if (node) {
				for (var i = 0; i < node.childNodes.length; i++) {
					if (node.childNodes[i].nodeName == name) {
						counter++;
					}
				}
			}
			return counter;
		},

		_getMaxColumns: function () {
			var maxCols = 0;
			var firstRow = this._tableToModify.rows[0];
			if (firstRow) {
				var cells = firstRow.cells; //childNodes;
				for (var i = 0; i < cells.length; i++) {
					maxCols += this._getColSpan(cells[i]);
				}
			}
			return maxCols;
		},

		_getRowSpan: function (oCell) {
			if (!oCell) return;
			return oCell.rowSpan > 0 ? oCell.rowSpan : 1;
		},

		_getColSpan: function (oCell) {
			if (!oCell) return;
			return oCell.colSpan > 0 ? oCell.colSpan : 1;
		},

		_deleteLastColumn: function () {
			var cells = [];
			if (this._getMaxColumns() == 1) {
				return false;
			}

			if (!this._selectedCell) {
				var previewTable = this._tablePreviewControl.get_previewTable();
				if (previewTable) {
					cells = previewTable.rows[0].cells;
					this._selectedCell = cells[cells.length - 1];
				}
			}

			cells = this._selectedCell.parentNode.cells;
			var lastCellIndex = cells.length - 1;
			if (this._selectedCell.cellIndex == lastCellIndex) {
				var result = this._tableLayoutBuilderEngine.executeCommand("deleteColumn");
				this.set_selectedCell(this._tableLayoutBuilderEngine._selectedTable.rows[0].cells[0]);
			}
			else {
				var cell = this._selectedCell;
				this.set_selectedCell(cells[lastCellIndex]);
				var result = this._tableLayoutBuilderEngine.executeCommand("deleteColumn");
				this.set_selectedCell(cell);
			}

			this._tablePreviewControl._updateTable(this._tableToModify, this._selectedCell);
			this._checkButtonAvailability();
		},

		_deleteLastRow: function () {
			if (this._rowsCount == 1) {
				return false;
			}

			var lasRowIndex = this._tableLayoutBuilderEngine._tableStateRowsCount - 1;
			if (this._tableLayoutBuilderEngine._selectedRowIndex == lasRowIndex) {
				var result = this._tableLayoutBuilderEngine.executeCommand("deleteRow");
				this.set_selectedCell(this._tableLayoutBuilderEngine._selectedTable.rows[0].cells[0]);
			}
			else {
				var cell = this._selectedCell;
				this.set_selectedCell(this._tableLayoutBuilderEngine._selectedTable.rows[lasRowIndex].cells[0]);
				var result = this._tableLayoutBuilderEngine.executeCommand("deleteRow");
				this.set_selectedCell(cell);
			}
			this._rowsCount -= 1;
			this._tablePreviewControl._updateTable(this._tableToModify, this._selectedCell);
			this._checkButtonAvailability();
		},

		_addNewColumn: function () {
			if (!this._tableLayoutBuilderEngine._selectedCell) return false;
			var result = this._tableLayoutBuilderEngine.executeCommand("insertColumnToTheRight");
			this._tablePreviewControl._updateTable(this._tableToModify, this._selectedCell);
			this._checkButtonAvailability();
		},

		_addNewRow: function () {
			if (!this._tableLayoutBuilderEngine._selectedCell) return false;
			var result = this._tableLayoutBuilderEngine.executeCommand("insertRowBelow");
			this._rowsCount += 1;
			this._tablePreviewControl._updateTable(this._tableToModify, this._selectedCell);
			this._checkButtonAvailability();
		},

		_increaseColSpan: function () {
			if (!this._tableLayoutBuilderEngine._selectedCell) return false;
			if (this._tableLayoutBuilderEngine.canMergeRight()) this._tableLayoutBuilderEngine.executeCommand("mergeRight");
			this._tablePreviewControl._updateTable(this._tableToModify, this._selectedCell);
			this._checkButtonAvailability();
		},

		_decreaseColSpan: function () {
			if (!this._tableLayoutBuilderEngine._selectedCell) return false;
			if (this._tableLayoutBuilderEngine._getColSpan(this._tableLayoutBuilderEngine._selectedCell) > 1) {
				var args = { rightColSpan: 1 }
				this._tableLayoutBuilderEngine.executeCommand("splitCellHorizontally", args);
			}

			this._tablePreviewControl._updateTable(this._tableToModify, this._selectedCell);
			this._checkButtonAvailability();
		},

		_increaseRowSpan: function () {
			if (!this._tableLayoutBuilderEngine._selectedCell) return false;
			if (this._tableLayoutBuilderEngine.canMergeDown()) this._tableLayoutBuilderEngine.executeCommand("mergeDown");
			this._tablePreviewControl._updateTable(this._tableToModify, this._selectedCell);
			this._checkButtonAvailability();
		},

		_decreaseRowSpan: function () {
			if (!this._tableLayoutBuilderEngine._selectedCell) return false;
			if (this._tableLayoutBuilderEngine._getRowSpan(this._tableLayoutBuilderEngine._selectedCell) > 1) {
				var args = { downRowSpan: 1 }
				this._tableLayoutBuilderEngine.executeCommand("splitCellVertically", args);
			}

			this._tablePreviewControl._updateTable(this._tableToModify, this._selectedCell);
			this._checkButtonAvailability();
		},

		_updateTable: function () {
			var table = this._update(this._tableToModify);
		},

		_update: function (table) {
			var widthValue = this._spinBoxWidth.get_value();
			if (widthValue == "") table.removeAttribute("width", false);
			else if (!this._isValueValid(widthValue)) return null;

			table.removeAttribute("width", false);
			table.style.width = widthValue ? parseFloat(widthValue) + this._widthUnit.get_selectedItem().get_value() : ""; //SAFARI - px

			var heightValue = this._spinBoxHeight.get_value();
			if (heightValue == "") table.removeAttribute("height", false);
			else if (!this._isValueValid(heightValue)) return null;

			table.removeAttribute("height", false);
			table.style.height = heightValue ? parseFloat(heightValue) + this._heightUnit.get_selectedItem().get_value() : ""; //SAFARI - px

			this._setAttribValue("align", this._alignmentSelectorTable.getAlign());

			var oSpacing = this._spinBoxCellSpacing.get_value();
			if (!isNaN(parseInt(oSpacing))) this._setAttribValue("cellSpacing", oSpacing >= 0 ? oSpacing : "", (oSpacing >= 0));

			var oPadding = this._spinBoxCellPadding.get_value();
			if (!isNaN(parseInt(oPadding))) this._setAttribValue("cellPadding", oPadding >= 0 ? oPadding : "", (oPadding >= 0));

			this._setCssPropertyValue(table, "backgroundColor", this._colorPicker.get_color(), "bgColor");

			var backgroundImage = this._imageCaller.get_value();
			backgroundImage = (backgroundImage) ? "url(" + backgroundImage + ")" : "";
			this._setCssPropertyValue(table, "backgroundImage", backgroundImage, "background");

			table.className = this._classSelector.get_value();
		},

		_isValueValid: function (value) {
			var valueInt = parseFloat(value, 10);
			if (!isNaN(valueInt)) {
				//NEW support for all css units
				var isValidPercent = (valueInt + '%' == value);
				var isValidPixel = (valueInt + 'px' == value.toLowerCase());
				var isValidEm = (valueInt + 'em' == value.toLowerCase());
				var isValidEx = (valueInt + 'ex' == value.toLowerCase());
				var isValidIn = (valueInt + 'in' == value.toLowerCase());
				var isValidCm = (valueInt + 'cm' == value.toLowerCase());
				var isValidMm = (valueInt + 'mm' == value.toLowerCase());
				var isValidPt = (valueInt + 'pt' == value.toLowerCase());
				var isValidPc = (valueInt + 'pc' == value.toLowerCase());
				var isValidNumber = (valueInt.toString() == value);

				if (isValidPercent || isValidPixel || isValidEm || isValidEx || isValidIn || isValidCm || isValidMm || isValidPt || isValidPc || isValidNumber) {
					return true;
				}
			}
			return false;
		},

		_convertIntToPixel: function (oVal) {
			var oNew = "" + oVal;

			if (oNew.indexOf("%") != -1) {
				return oNew;
			}
			else {
				var arMatch = oNew.match(/(em|ex|px|in|cm|mm|pt|pc)$/); //NEW support for all css units
				oNew = parseFloat(oNew);
				if (!isNaN(oNew)) {
					oNew = (arMatch) ? oNew + arMatch[0] : oNew + "px"; //NEW support for all css units
					return oNew;
				}
			}
			return oVal;
		},

		_setAttribValue: function (attribName, oVal, forceVal) {
			if (oVal || (true == forceVal))//cellSpacing or cellPadding have by default 1 so we might want to set it to 0
			{
				this._tableToModify.setAttribute(attribName, oVal);
			}
			else {
				this._tableToModify.removeAttribute(attribName, false);
			}
		},

		_setCssPropertyValue: function (element, cssProperty, value, attribute) {
			if (!element || !cssProperty) return;
			if (attribute) element.removeAttribute(attribute);
			element.style[cssProperty] = value;
		},

		set_selectedCell: function (value) {
			this._selectedCell = value;
			this._tableLayoutBuilderEngine.set_selectedCell(value);
			if (this._tableToModify != null) {
				this._tablePreviewControl._updateTable(this._tableToModify, this._selectedCell);
			}

			//NEW always check _checkButtonAvailability when you set selected cell
			this._checkButtonAvailability();
		},

		_cssValueSelected: function (oTool, args) {
			if (!oTool) return;
			var commandName = oTool.get_name();

			if ("ApplyClass" == commandName) {
				var attribValue = oTool.get_selectedItem();
				oTool.updateValue(attribValue);
			}
		},

		set_tableWizardControl: function (value) {
			this._tableWizardDialog = value;
		},

		dispose: function () {
			$(this._styleBuilder).off("click", this._styleBuilderBtnClickDelegate);

			this._clientParameters = null;
			this._currentRowSpan = null;
			this._currentColSpan = null;
			this._spinBoxColumns = null;
			this._spinBoxColumnSpan = null;
			this._spinBoxRows = null;
			this._spinBoxRowSpan = null;
			this._selectedCell = null;
			this._selectedCellIndex = null;
			this._victimColumns = null;
			this._victimRows = null;
			this._rowsCount = null;
			this._tableToModify = null;
			this._tablePreviewControl = null;
			this._tableWizardDialog = null;
			this._tableLayoutBuilderEngine = null;

			Telerik.Web.UI.Widgets.TableProperties.callBaseMethod(this, "dispose");
		}
	}
	Telerik.Web.UI.Widgets.TableProperties.registerClass('Telerik.Web.UI.Widgets.TableProperties', Telerik.Web.UI.RadWebControl, Telerik.Web.IParameterConsumer);
</script>

<div class="redInlineBlock redBlockLeft redTableWizard">
	<span class="rfdAspLabel rfdAspTitle redInlineBlock">
		<script type="text/javascript">document.write(localization["Preview"]);</script>
	</span>
	<div id="TableProperties_PreviewTableHolder" class="reTablePropertiesPreviewTableHolder">
		<!-- / -->
	</div>
	<div class="redRow">
		<div class="redInlineBlock redMiddle redBlockLeft">
			<span class="rfdAspLabel redInlineBlock">
				<script type="text/javascript">document.write(localization["Columns"]);</script>
			</span>
			<span class="reInsertSelectSpinBoxWrapper redInlineBlock redMiddle">
				<tools:EditorSpinBox ID="SpinBoxColumns" VisibleInput="false" runat="server" />
			</span>
		</div>
		<div class="redInlineBlock redMiddle redBlockRight">
			<span class="rfdAspLabel redInlineBlock">
				<script type="text/javascript">document.write(localization["ColumnSpan"]);</script>
			</span>
			<span class="reInsertSelectSpinBoxWrapper redInlineBlock redMiddle">
				<tools:EditorSpinBox ID="SpinBoxColumnSpan" VisibleInput="false" runat="server" />
			</span>
		</div>
	</div>
	<div class="redRow">
		<div class="redInlineBlock redMiddle redBlockLeft">
			<span class="rfdAspLabel redInlineBlock">
				<script type="text/javascript">document.write(localization["Rows"]);</script>
			</span>
			<span class="reInsertSelectSpinBoxWrapper redInlineBlock redMiddle">
				<tools:EditorSpinBox ID="SpinBoxRows" VisibleInput="false" runat="server" />
			</span>
		</div>
		<div class="redInlineBlock redMiddle redBlockRight">
			<span class="rfdAspLabel redInlineBlock">
				<script type="text/javascript">document.write(localization["RowSpan"]);</script>
			</span>
			<span class="reInsertSelectSpinBoxWrapper redInlineBlock redMiddle">
				<tools:EditorSpinBox ID="SpinBoxRowSpan" VisibleInput="false" runat="server" />
			</span>
		</div>
	</div>
</div>

<div class="redInlineBlock redBlockRight redTableWizard">
	<div class="redSection redSectionNoBorder">

		<div class="redRow">
			<label class="redLabel">
				<script type="text/javascript">document.write(localization["Height"]);</script>
			</label>
			<span class="redInlineBlock redMiddle">
				<tools:EditorSpinBox ID="SpinBoxTableHeight" runat="server"></tools:EditorSpinBox>
			</span>
			<span class="redInlineBlock redMiddle redDropDownShort reUnitDropdown">
				<telerik:RadDropDownList runat="server" ID="HeightUnit">
					<Items>
						<telerik:DropDownListItem Text="px" Value="px" />
						<telerik:DropDownListItem Text="%" Value="%" />
						<telerik:DropDownListItem Text="pt" Value="pt" />
					</Items>
				</telerik:RadDropDownList>
			</span>
		</div>

		<div class="redRow">
			<label class="redLabel">
				<script type="text/javascript">document.write(localization["Width"]);</script>
			</label>
			<span class="redInlineBlock redMiddle">
				<tools:EditorSpinBox ID="SpinBoxTableWidth" runat="server"></tools:EditorSpinBox>
			</span>
			<span class="redInlineBlock redMiddle redDropDownShort reUnitDropdown">
				<telerik:RadDropDownList runat="server" ID="WidthUnit">
					<Items>
						<telerik:DropDownListItem Text="px" Value="px" />
						<telerik:DropDownListItem Text="%" Value="%" />
						<telerik:DropDownListItem Text="pt" Value="pt" />
					</Items>
				</telerik:RadDropDownList>
			</span>
		</div>

		<div class="redRow">
			<label class="redLabel">
				<script type="text/javascript">document.write(localization["CellSpacing"]);</script>
			</label>
			<span class="redInlineBlock redMiddle">
				<tools:EditorSpinBox ID="SpinBoxTableCellSpacing" runat="server"></tools:EditorSpinBox>
			</span>
		</div>

		<div class="redRow">
			<label class="redLabel">
				<script type="text/javascript">document.write(localization["CellPadding"]);</script>
			</label>
			<span class="redInlineBlock redMiddle">
				<tools:EditorSpinBox ID="SpinBoxTableCellSPadding" runat="server"></tools:EditorSpinBox>
			</span>
		</div>

		<div class="redRow">
			<label class="redLabel">
				<script type="text/javascript">document.write(localization["Alignment"]);</script>
			</label>
			<span class="redInlineBlock redMiddle redToolSplitButton">
				<tools:AlignmentSelector ID="TableAlignment" runat="server"></tools:AlignmentSelector>
			</span>
		</div>

		<div class="redRow">
			<label class="redLabel">
				<script type="text/javascript">document.write(localization["BackgroundColor"]);</script>
			</label>
			<span class="redInlineBlock redMiddle redToolSplitButton">
				<tools:ColorPicker ID="ColorPicker1" runat="server"></tools:ColorPicker>
			</span>
		</div>

		<div class="redRow">
			<label class="redLabel">
				<script type="text/javascript">document.write(localization["CssClass"])</script>
			</label>
			<span class="redInlineBlock redMiddle redToolSplitButton">
				<tools:ApplyClassDropDown ID="TableCssClass" runat="server" />
			</span>
		</div>

		<div class="redRow">
			<label class="redLabel">
				<script type="text/javascript">document.write(localization["BackgroundImage"]);</script>
			</label>
			<span class="redInlineBlock redMiddle redToolSplitButton reSingleIcon">
				<tools:ImageDialogCaller ID="TableBgImage" runat="server"></tools:ImageDialogCaller>
			</span>
		</div>

		<div class="redRow">
			<label class="redLabel">
			</label>
			<span class="redInlineBlock redMiddle redToolSplitButton">
				<span class="redStyleBuilderButton">
					<button id="StyleBuilderBtnTP">More Table Styling...</button>
				</span>
			</span>
		</div>

	</div>
</div>