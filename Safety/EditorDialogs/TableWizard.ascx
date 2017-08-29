<%@ Control Language="C#" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI.Widgets" TagPrefix="widgets" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI.Dialogs" TagPrefix="dialogs" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI.Editor.DialogControls" TagPrefix="dc" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<script type="text/javascript">
	Type.registerNamespace("Telerik.Web.UI.Widgets");

	Telerik.Web.UI.Widgets.TablePreview = function (element) {
		Telerik.Web.UI.Widgets.TablePreview.initializeBase(this);

		this._previewHolder = null;
		this._previewTable = null;
		this._editedCells = [];
		this._selectedCell = null;
		this._selectedCells = [];
		this._allowMultiCellSelection = false;
		this._selectAllButton = null;
		this._selectAllHolder = null;
	}

	Telerik.Web.UI.Widgets.TablePreview.prototype = {
		initialize: function () {
			Telerik.Web.UI.Widgets.TablePreview.callBaseMethod(this, "initialize");
		},

		_configureChildren: function () {
			var dummyTable = document.createElement("table");
			dummyTable.className = "tableDesign";
			this._previewHolder.appendChild(dummyTable);
			this._previewTable = dummyTable;

			//select all element
			var divSelectAllHolder = this._selectAllHolder = document.createElement("div");
			var btnSelectAll = this._selectAllButton = document.createElement("button");

			divSelectAllHolder.style.display = "none";
			divSelectAllHolder.className = "reTableWizardSelectAllBtnWrapper";

			var buttonText = localization["SelectAll"];
			btnSelectAll.title = buttonText;
			btnSelectAll.innerHTML = buttonText;
			btnSelectAll.className = "rfdFlatButton rfdSkinnedButton";

			divSelectAllHolder.appendChild(btnSelectAll);
			this._previewHolder.appendChild(divSelectAllHolder);
			$addHandlers(btnSelectAll, { "click": this._handleSelectAllButtonClick }, this);
		},

		_updateTable: function (modelTable, selectedCell) {
			var newTable = document.createElement("table");
			newTable.className = "tableDesign";
			newTable.cellPadding = 1;
			newTable.cellSpacing = 1;

			for (var i = 0; i < modelTable.rows.length; i++) {
				var currentRow = modelTable.rows[i];
				var newRow = newTable.insertRow(-1);//newTable.rows.length
				for (var j = 0; j < currentRow.cells.length; j++) {
					var currentCell = currentRow.cells[j];
					var newCell = newRow.insertCell(-1);//newRow.cells.length

					newCell.rowSpan = currentCell.rowSpan;
					newCell.colSpan = currentCell.colSpan;
					if (currentCell == selectedCell) {
						this._selectCell(newCell);
					}
					else {
						this._deSelectCell(newCell);
					}

					newCell.theTablePreviewControl = this;
					$addHandlers(newCell, { "click": this._handleCellClick }, this);
					newCell.innerHTML = "&nbsp;&nbsp;";
				}
			}
			this._previewTable.parentNode.replaceChild(newTable, this._previewTable);
			this._previewTable = newTable;
		},

		_handleSelectAllButtonClick: function (ev) {
			if (this._allowMultiCellSelection) {
				var rows = this._previewTable.rows;
				var rl = rows.length;
				for (var i = 0; i < rl; i++) {
					var cells = rows[i].cells;
					var cl = cells.length;
					for (var j = 0; j < cl; j++) {
						this._selectCell(cells[j]);
					}
				}
			}
			ev.preventDefault();
		},

		_handleCellClick: function (e) {
			var cell = null;
			if (!e)
				cell = window.event.srcElement;
			else
				cell = e.target;

			if (this._allowMultiCellSelection && e.ctrlKey) {
				/**
				 *
				 * If the control key is pressed a multiple selection
				 * is being done.
				 *
				 */
				if (this._isCellSelected(cell)) {
					this._deSelectCell(cell);
				}
				else {
					this._selectCell(cell);
				}
			}
			else {
				this._changeSelectedCell(cell);
			}
		},

		_changeSelectedCell: function (cell) {
			this.set_editedCells(this.get_selectedCells());
			var isCellSelected = this._isCellSelected(cell);
			var isMultiCellSelection = this._isMultiCellSelection();
			this.deSelectAllCells();
			if (!isCellSelected || isMultiCellSelection) {
				this._selectCell(cell);
			}

			if (this._onSelectedCellChanged) {
				this._onSelectedCellChanged();
			}
		},

		_getCellIndex: function (oCell) {

			var selCol = oCell ? (oCell.cellIndex) : 0;

			if ($telerik.isSafari) //SAFARI
			{
				var oP = oCell.parentNode;
				for (var i = 0; i < oP.cells.length; i++) {
					if (oCell == oP.cells[i]) {
						selCol = i;
						break;
					}
				}
			}
			return selCol;
		},

		_selectCell: function (cell) {
			this._selectedCell = cell;

			var rowIndex = cell.parentNode.rowIndex;
			var cellIndex = this._getCellIndex(cell);//cell.cellIndex; TEKI - SAFARI support.

			if (typeof (this._selectedCells[rowIndex]) == 'undefined') {
				this._selectedCells[rowIndex] = [];
			}

			this._selectedCells[rowIndex][cellIndex] = cell;

			//TODO do this with $telerik.addCssClasses
			cell.className = "selectedCell";
		},

		_deSelectCell: function (cell) {
			if (!cell) return;
			if (this._isCellSelected(cell)) {
				var rowIndex = cell.parentNode.rowIndex;
				var cellIndex = this._getCellIndex(cell);//cell.cellIndex; TEKI - SAFARI support. Defined in Common.js
				if (rowIndex >= 0 && cellIndex >= 0) {
					this._selectedCells[rowIndex][cellIndex] = null;
				}
				if (cell == this._selectedCell) {
					this._selectedCell = null;
				}
			}

			cell.className = "";
		},

		deSelectAllCells: function () {
			var selectedCells = this.get_selectedCells();
			for (var i = 0; i < selectedCells.length; i++) {
				this._deSelectCell(selectedCells[i]);
			}

			this._selectedCells = [];
		},

		_isCellSelected: function (cell) {
			return (cell && cell.className == "selectedCell"); // Or use the this.selectedCells Array ?
		},

		_isMultiCellSelection: function () {
			return (this.get_selectedCells().length > 1);
		},

		get_previewHolder: function () {
			return this._previewHolder;
		},

		set_previewHolder: function (value) {
			this._previewHolder = value;

			if (this._previewHolder) this._configureChildren();
		},

		set_editedCells: function (editedCells) {
			this._editedCells = editedCells;
		},

		get_editedCells: function () {
			return this._editedCells;
		},

		get_previewTable: function () {
			return this._previewTable;
		},

		get_selectedCells: function () {
			var selectedCells = [];
			for (var rowIndex in this._selectedCells) {
				for (var cellIndex in this._selectedCells[rowIndex]) {
					selectedCells.push(this._selectedCells[rowIndex][cellIndex]);
				}
			}
			return selectedCells;
		},

		get_selectedCell: function () {
			var selectedCells = this.get_selectedCells();
			if (selectedCells && selectedCells.length > 0) {
				return selectedCells[selectedCells.length - 1];
			}
			return null;
		},

		set_allowMultiCellSelection: function (value) {
			this._selectAllHolder.style.display = (value) ? "" : "none";
			this._allowMultiCellSelection = value;
		},

		_disposeChildEvents: function () {
			if (this._previewTable) {
				for (var i = 0; i < this._previewTable.rows.length; i++) {
					var currentRow = this._previewTable.rows[i];
					for (var j = 0; j < currentRow.cells.length; j++) {
						$clearHandlers(currentRow.cells[j]);
					}
				}
			}
		},

		dispose: function () {
			this._disposeChildEvents();

			this._previewHolder = null;
			this._previewTable = null;
			this._editedCells = [];
			this._selectedCell = null;
			this._selectedCells = [];

			Telerik.Web.UI.Widgets.TablePreview.callBaseMethod(this, "dispose");
		}
	}

	Telerik.Web.UI.Widgets.TablePreview.registerClass('Telerik.Web.UI.Widgets.TablePreview', Sys.Component);

	Telerik.Web.UI.Widgets.TableWizard = function (element) {
		Telerik.Web.UI.Widgets.TableWizard.initializeBase(this, [element]);

		this._clientParameters = null;
		this._insertButton = null;
		this._cancelButton = null;
		this._table = null;
		this._originalSelectedCell = null;
		this._currentlyEditedCell = null;
		this._originalTableClone = null;
		this._selectedTabIndex = 0;

		this._selectedCell = null;
		this._selectedCellIndex = -1;

		this._accessibleTableControl = null;
		this._cellPropertiesControl = null;
		this._tablePropertiesControl = null;
		this._tableLayoutsControl = null;
		this._tablePreviewControl = null;

		//Due to dialog caching make and difference in sequence of initialization we will use a trick to call
		this._isLoaded = false;
	}

	Telerik.Web.UI.Widgets.TableWizard.prototype = {
		initialize: function () {
			Telerik.Web.UI.Widgets.TableWizard.callBaseMethod(this, 'initialize');
			this._setupChildren();
		},

		//Called also when a dilog is cached
		clientInit: function (clientParameters) {
			this._clientParameters = clientParameters;
			this._table = this._clientParameters.originalTable;
			this._originalTableClone = this._clientParameters.tableToModify;
			this._originalSelectedCell = this._clientParameters.cellToModify;

			this._initializeControl();
		},

		_initializeControl: function () {
			var params = this._clientParameters;

			this._selectedTabIndex = params.selectedTabIndex || 0;
			this._tab.set_selectedIndex(this._selectedTabIndex);

			//New - initialize manually subcontrols
			this._tablePropertiesControl.clientInit(params);
			this._cellPropertiesControl.clientInit(params);
			this._tableLayoutsControl.clientInit(params);
			this._accessibleTableControl.clientInit(params);

			this._loadSelectedTab();
		},

		_setupChildren: function () {
			this._tablePropertiesControl = $find("tablePropertiesPanel");
			this._cellPropertiesControl = $find("cellPropertiesPanel");
			this._tableLayoutsControl = $find("tableLayoutsPanel");
			this._accessibleTableControl = $find("accessibilityPanel");

			this._insertButton = $get("InsertButton");
			this._cancelButton = $get("CancelButton");
			this._tab = $find("TableWizardTab");

			this._initializeChildEvents();
		},

		_initializeChildEvents: function () {
			$addHandlers(this._insertButton, { "click": this._insertClickHandler }, this);
			$addHandlers(this._cancelButton, { "click": this._cancelClickHandler }, this);

			this.tabChangedHandler = Function.createDelegate(this, this._tabChangedHandler);
			this._tab.add_tabSelected(this.tabChangedHandler);
		},

		_tabChangedHandler: function (sender, args) {
			var selectedTab = sender.get_selectedIndex();
			this._initTabControl(selectedTab);
			this._selectedTabIndex = selectedTab;
		},

		_loadSelectedTab: function () {
			this._initTabControl(this._selectedTabIndex);
		},

		_initTabControl: function (selectedTab) {
			switch (selectedTab) {
				case 0:
					this._initTableProperties();
					break;
				case 1:
					this._initCellProperties();
					break;
				case 2:
					this._initTableLayouts();
					break;
				case 3:
					this._initAccessibleTable();
					break;
			}
		},

		_removeEmptyThead: function (table) {
			if (!table) return;
			var thead = table.tHead;
			if (thead && thead.innerHTML.match(/^\s*$/)) {
				table.removeChild(thead);
			}
		},

		_getModifiedTable: function () {
			var table = this._table.cloneNode(true);

			this._accessibleTableControl.set_tableToModify(table);
			this._originalTableClone = this._accessibleTableControl._updateTable();

			this._tablePropertiesControl._tableToModify = this._originalTableClone;
			this._tablePropertiesControl._updateTable();

			this._tableLayoutsControl.set_tableToModify(this._originalTableClone);
			var modifiedTable = this._tableLayoutsControl._updateTable();
			if (modifiedTable) this._originalTableClone = modifiedTable;
			this._removeEmptyThead(this._originalTableClone);

			return this._originalTableClone;
		},

		_insertClickHandler: function (e) {
			if (!this._saveLastEditedCellProperties(false)) {
				return;
			}

			var modifiedTable = this._getModifiedTable();

			Telerik.Web.UI.Dialogs.CommonDialogScript.get_windowReference().close({
				commandName: "TableWizard",
				tableToModify: modifiedTable,
				styleSheetToImport: this._tableLayoutsControl._selectedStyleSheet,
				styleSheetToRemove: this._tableLayoutsControl._styleSheetToRemove
			});
		},

		_cancelClickHandler: function (e) {
			if (this._table && this._table.parentNode) //TEKI: According to SAFARI parentNode could be null when Cancel is pressed
				this._table.parentNode.replaceChild(this._originalTableClone, this._table);
			this._table = this._originalTableClone;

			Telerik.Web.UI.Dialogs.CommonDialogScript.get_windowReference().close();
		},

		/////////////////////////////////////////////////////////
		_getCurrentlyEditedCell: function () {
			return this._currentlyEditedCell;
		},

		_onCellPropertiesSelectedMultipleCells: function () {
			this._tableLayoutsControl._clear();
		},

		_synchronizeSelectedCell: function () {
			var cellProperties = this._cellPropertiesControl;
			var tablePreview = cellProperties._tablePreviewControl;
			var previewSelectedCell = tablePreview.get_selectedCell();
			var previewTable = tablePreview.get_previewTable();

			var previewTableRows = previewTable.rows;

			this._selectedCell = null;
			this._selectedCellIndex = -1;

			for (var i = 0; i < previewTableRows.length; i++) {
				var previewTableCells = previewTableRows[i].cells;
				for (var j = 0; j < previewTableCells.length; j++) {
					if (previewTableCells[j] == previewSelectedCell) {
						this._selectedCell = (this._table.rows.length > 0) ? this._table.rows[i].cells[j] : null;

						this._selectedCellIndex = j;
						return;
					}
				}
			}
		},

		_synchronizeSelection: function (selectedCell, selectedIndex) {
			this._selectedCell = selectedCell;
			this._selectedCellIndex = selectedIndex;
		},

		/**
		*
		* For a given preview cell return the model cell
		*
		*/
		_getModifyCellForPreviewCell: function (previewCell) {
			if (!previewCell) return null;
			var rowIndex = previewCell.parentNode.rowIndex;
			var cellIndex = this._cellPropertiesControl._tablePreviewControl._getCellIndex(previewCell); // PreviewCell.cellIndex; TEKI - SAFARI support! Defined in Common.js

			return (this._table.rows.length > 0) ? this._table.rows[rowIndex].cells[cellIndex] : null;
		},

		_getModifyCellsForPreviewCells: function (previewCells) {
			var modifyCells = [];

			for (var i = 0; i < previewCells.length; i++) {
				modifyCells.push(this._getModifyCellForPreviewCell(previewCells[i]));
			}

			return modifyCells;
		},

		_saveLastEditedCellProperties: function (saveSelectedCell) {
			if (this._currentlyEditedCell) {
				var cellProperties = this._cellPropertiesControl;
				var selectedCells = cellProperties._tablePreviewControl.get_selectedCells()

				if (!cellProperties._updateMultiple(this._getModifyCellsForPreviewCells(selectedCells))) {
					return false;
				}
				if (!saveSelectedCell) {
					this._selectedCell = null;
					this._selectedCellIndex = -1;
				}
			}
			return true;
		},

		_initTableLayouts: function () {
		},

		_initCellProperties: function () {
			var tableWizard = this;
			var cellProperties = tableWizard._cellPropertiesControl;
			var tablePreviewControl = cellProperties._tablePreviewControl;

			tablePreviewControl._onSelectedCellChanged = function () {
				tableWizard._onCellPropertiesSelectedCellChanged();
			};

			tablePreviewControl.deSelectAllCells();
			tablePreviewControl.set_allowMultiCellSelection(true);

			if (!this._originalSelectedCell) {
				if (!this._selectedCell)
				this._initSelectedCell();
			}
			else {
				this._selectedCell = this._originalSelectedCell;
				//Clear the originally selected cell.
				this._originalSelectedCell = null;
			}

			tablePreviewControl._updateTable(this._table, this._selectedCell);

			this._currentlyEditedCell = this._selectedCell;

			this._cellPropertiesControl.set_cellToModify(this._currentlyEditedCell);
		},

		_initAccessibleTable: function () {
			//Clear the originally selected cell.
			this._originalSelectedCell = null;

			this._saveLastEditedCellProperties(true);
			this._accessibleTableControl.set_tableToModify(this._table.cloneNode(true));
		},

		_initSelectedCell: function () {
			if (this._table.rows.length > 0) {
				if (this._table.rows[0].cells.length > 0) {
					this._selectedCell = this._table.rows[0].cells[0];
				}
			}
		},

		_initTableProperties: function () {
			//Clear the originally selected cell.
			this._originalSelectedCell = null;
			this._saveLastEditedCellProperties(true);

			this._tablePropertiesControl.set_tableWizardControl(this);

			//NEW _initSelectedCell on every opening of the Table Wizard
			if (!this._selectedCell || this._tablePropertiesControl._selectedCell != this._selectedCell)
				this._initSelectedCell();

			this._tablePropertiesControl.set_selectedCell(this._selectedCell);
		},

		_onCellPropertiesSelectedCellChanged: function () {
			// Update the last edited cells
			var cellProperties = this._cellPropertiesControl;

			var editedCells = cellProperties._tablePreviewControl.get_editedCells();
			cellProperties._updateMultiple(this._getModifyCellsForPreviewCells(editedCells));

			this._synchronizeSelectedCell();

			if (this._selectedCell) {
				this._currentlyEditedCell = this._selectedCell;
				cellProperties._loadData(this._selectedCell);
			}
		},

		get_rippleZonesConfiguration: function() {
			return [
				{
					element: document.body,
					rippleConfigurations: [
						{
							containerSelector: '.reToolIcon, .reDropdown'
						}
					]
				}
			];
		},

		dispose: function () {
			$clearHandlers(this._insertButton);
			$clearHandlers(this._cancelButton);

			this._tab.remove_tabSelected(this.tabChangedHandler);

			this._clientParameters = null;
			this._insertButton = null;
			this._cancelButton = null;
			this._table = null;
			this._originalSelectedCell = null;
			this._currentlyEditedCell = null;
			this._originalTableClone = null;
			this._selectedCell = null;
			this._tablePreviewControl = null;

			this._accessibleTableControl = null;
			this._cellPropertiesControl = null;
			this._tablePropertiesControl = null;
			this._tableLayoutsControl = null;

			Telerik.Web.UI.Widgets.TableWizard.callBaseMethod(this, 'dispose');
		}
	}

	Telerik.Web.UI.Widgets.TableWizard.registerClass('Telerik.Web.UI.Widgets.TableWizard', Telerik.Web.UI.RadWebControl, Telerik.Web.IParameterConsumer);
</script>
<!-- table wizard -->
<div class="redMainContainer redTabStrip">
	<div class="redTableWizard">

		<telerik:RadTabStrip ShowBaseLine="true" ID="TableWizardTab" runat="server" SelectedIndex="0" MultiPageID="dialogMultiPage">
			<Tabs>
				<telerik:RadTab Text="Table Properties" Value="Tab1HeaderText">
				</telerik:RadTab>
				<telerik:RadTab Text="Cell Properties" Value="Tab2HeaderText">
				</telerik:RadTab>
				<telerik:RadTab Text="Table Layouts" Value="Tab3HeaderText">
				</telerik:RadTab>
				<telerik:RadTab Text="Accessibility" Value="Tab4HeaderText">
				</telerik:RadTab>
			</Tabs>
		</telerik:RadTabStrip>

		<div class="redWrapper">
			<telerik:RadMultiPage ID="dialogMultiPage" runat="server" SelectedIndex="0">
				<telerik:RadPageView ID="tablePropertiesRow" runat="server">
					<dc:TableProperties ID="tablePropertiesPanel" runat="server"></dc:TableProperties>
				</telerik:RadPageView>
				<telerik:RadPageView ID="cellPropertiesRow" runat="server">
					<dc:CellProperties ID="cellPropertiesPanel" runat="server"></dc:CellProperties>
				</telerik:RadPageView>
				<telerik:RadPageView ID="tableLayoutsRow" runat="server">
					<dc:TableLayouts ID="tableLayoutsPanel" runat="server"></dc:TableLayouts>
				</telerik:RadPageView>
				<telerik:RadPageView ID="accessibilityRow" runat="server">
					<dc:AccessibleTable ID="accessibilityPanel" runat="server"></dc:AccessibleTable>
				</telerik:RadPageView>
			</telerik:RadMultiPage>

		</div>
		<div class="redActionButtonsWrapper redActionButtonsAbsoluteWrapper redActionButtonsWrapperExternal" id="controlButtonsRow" runat="server">
			<button type="button" id="InsertButton" class="rfdPrimaryButton rfdFlatButton">
				<script type="text/javascript">
					setInnerHtml("InsertButton", localization["OK"]);
				</script>
			</button>
			<button type="button" id="CancelButton" class="rfdFlatButton">
				<script type="text/javascript">
					setInnerHtml("CancelButton", localization["Cancel"]);
				</script>
			</button>

		</div>
	</div>
</div>