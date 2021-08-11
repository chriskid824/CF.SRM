import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { SrmRfqService } from '../../../business/srm/srm-rfq.service';
import { SrmPriceService } from '../../../business/srm/srm-price.service';
import { ActivatedRoute } from '@angular/router';
import { Rfq } from '../model/rfq';
import { StorageService } from '../../../services/storage.service';
import datepickerFactory from 'jquery-datepicker';

declare const $: any;
datepickerFactory($);
@Component({
  selector: 'app-price',
  templateUrl: './price.component.html',
  styleUrls: ['./price.component.less']
})
export class PriceComponent implements OnInit {
  matnrList: FormGroup = new FormGroup({});
  selectedMatnr;
  matnrs = [];
  rfq: Rfq;
  rfqId;
  MaterialList;
  ProcessList;
  SurfaceList;
  OtherList;

  canModify = true;

  radioValue;

  //grid
  columnDefs_matnr
  columnDefs_material;
  columnDefs_process;
  columnDefs_surface;
  columnDefs_other;
  columnDefs_inforecord;
  defaultColDef;
  rowSelection = "multiple";
  components;

  rowData_matnr;
  rowData_material;
  rowData_process;
  rowData_surface;
  rowData_other;
  rowData_inforecord;

  gridApi_inforecord;
  columnApi_inforecord;;
  //gird

  constructor(private activatedRoute: ActivatedRoute
    , private _formBuilder: FormBuilder
    , private _srmRfqService: SrmRfqService
    , private _srmPriceService: SrmPriceService
    , private _storageService: StorageService  ) {
    //this.activatedRoute.queryParams.subscribe(params => {
    //  this.rfqId = params['id'];
    //  //this.rfqId = 12;
    //});
    this.activatedRoute.params.subscribe((params) => this.rfqId = params['id']);

    //grid 
    this.defaultColDef = {
      filter: "agTextColumnFilter",
      allowedAggFuncs: ['sum', 'min', 'max'],
      enableValue: true,
      enableRowGroup: true,
      enablePivot: true,
      resizable: true,
      rowSelection: "multiple",
      wrapText: true,
    };
  //gird


    this.components = { datePicker: getDatePicker() };

    this.columnDefs_inforecord = [
      {
        headerName: "供應商",
        field: "vendorName",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
        headerCheckboxSelection: true,
        checkboxSelection: true,
      },
      {
        headerName: "A",
        field: "atotal",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "100px",
      },
      {
        headerName: "B",
        field: "btotal",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "100px",
      },
      {
        headerName: "C",
        field: "ctotal",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "100px",
      },
      {
        headerName: "D",
        field: "dtotal",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "100px",
      },
      {
        headerName: "總計金額",
        field: "price",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
        editable: this.canModify,
/*        stopEditingWhenCellsLoseFocus:true,*/
      },
      {
        headerName: "價格單位",
        field: "unit",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
        editable: this.canModify,
      },
      {
        headerName: "採購群組",
        field: "ekgry",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
        editable: this.canModify,
      },
      {
        headerName: "計畫交貨時間",
        field: "leadTime",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
        editable: this.canModify,
      },
      {
        headerName: "標準採購數量",
        field: "standQty",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
        editable: this.canModify,
      },
      {
        headerName: "最小採購數量",
        field: "minQty",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
        editable: this.canModify,
      },
      {
        headerName: "稅碼",
        field: "taxcode",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
        editable: this.canModify,
      },
      {
        headerName: "生效日期",
        field: "effectiveDate",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
        editable: this.canModify,
        cellEditor: 'datePicker',
      },
      {
        headerName: "有效日期",
        field: "expirationDate",
        width: "150px",
        editable: this.canModify,
        cellEditor: 'datePicker',
      }
    ]
  }

  ngOnInit(): void {
    this.matnrList = this._formBuilder.group({
      selectedMatnr: [null]
    });
    this.init();
    this.initGrid();
  }
  init() {
    this._srmRfqService.GetRfqData(this.rfqId).subscribe(result => {
      this.rfq = result;
      this.matnrs = [];
      console.log(result);
      console.log(this.rfq);
      this.rfq.m.forEach(row => this.matnrs.push({ label: row.srmMatnr, value: row.matnrId }));
      this.matnrList.setValue({ selectedMatnr: this.matnrs[0].value });
      //this.radioValue = this.matnrs[0].value;
    });
  }


  initGrid() {
    this.columnDefs_matnr = [
      {
        headerName: "料號",
        field: "matnr",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "材質規格",
        field: "material",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "成品尺寸",
        field: "volume",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "重量",
        field: "weight",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "機種",
        field: "machineName",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      }
    ]
    this.columnDefs_material = [
      {
        headerName: "供應商",
        field: "vendorName",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "材料名稱",
        field: "mMaterial",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "材料單價",
        field: "mPrice",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "長",
        field: "length",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "寬",
        field: "width",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "厚",
        field: "height",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "密度",
        field: "density",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "重量",
        field: "weight",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "材料成本",
        field: "mCostPrice",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "備註",
        field: "note",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      }
    ]
    this.columnDefs_process = [
      {
        headerName: "供應商",
        field: "vendorName",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "機台",
        field: "pMachine",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "工序",
        field: "pProcessNum",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "工時(時)",
        field: "pHours",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "單價(時)",
        field: "pPrice",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "小計",
        field: "subTotal",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "備註",
        field: "pNote",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      }
    ]
    this.columnDefs_surface = [
      {
        headerName: "供應商",
        field: "vendorName",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",

      },
      {
        headerName: "工序",
        field: "sProcess",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "數量",
        field: "sTimes",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "單價",
        field: "sPrice",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "小計",
        field: "subTotal",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "計價方式",
        field: "method",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "備註",
        field: "sNote",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      }
    ]
    this.columnDefs_other = [
      {
        headerName: "供應商",
        field: "vendorName",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "項目",
        field: "oItem",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "說明",
        field: "oDescription",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "單價",
        field: "oPrice",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      },
      {
        headerName: "備註",
        field: "oNote",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "240px",
      }
    ]

    //this.columnDefs_inforecord = [
    //  {
    //    headerName: "供應商",
    //    field: "vendorName",
    //    enableRowGroup: true,
    //    cellClass: "show-cell",
    //    width: "150px",
    //    headerCheckboxSelection: true,
    //    checkboxSelection: true,
    //  },
    //  {
    //    headerName: "A",
    //    field: "atotal",
    //    enableRowGroup: true,
    //    cellClass: "show-cell",
    //    width: "100px",
    //  },
    //  {
    //    headerName: "B",
    //    field: "btotal",
    //    enableRowGroup: true,
    //    cellClass: "show-cell",
    //    width: "100px",
    //  },
    //  {
    //    headerName: "C",
    //    field: "ctotal",
    //    enableRowGroup: true,
    //    cellClass: "show-cell",
    //    width: "100px",
    //  },
    //  {
    //    headerName: "D",
    //    field: "dtotal",
    //    enableRowGroup: true,
    //    cellClass: "show-cell",
    //    width: "100px",
    //  },
    //  {
    //    headerName: "總計金額",
    //    field: "price",
    //    enableRowGroup: true,
    //    cellClass: "show-cell",
    //    width: "150px",
    //    editable: this.canModify,
    //  },
    //  {
    //    headerName: "價格單位",
    //    field: "unit",
    //    enableRowGroup: true,
    //    cellClass: "show-cell",
    //    width: "150px",
    //    editable: this.canModify,
    //  },
    //  {
    //    headerName: "採購群組",
    //    field: "ekgry",
    //    enableRowGroup: true,
    //    cellClass: "show-cell",
    //    width: "150px",
    //    editable: this.canModify,
    //  },
    //  {
    //    headerName: "計畫交貨時間",
    //    field: "leadTime",
    //    enableRowGroup: true,
    //    cellClass: "show-cell",
    //    width: "150px",
    //    editable: this.canModify,
    //  },
    //  {
    //    headerName: "標準採購數量",
    //    field: "standQty",
    //    enableRowGroup: true,
    //    cellClass: "show-cell",
    //    width: "150px",
    //    editable: this.canModify,
    //  },
    //  {
    //    headerName: "最小採購數量",
    //    field: "minQty",
    //    enableRowGroup: true,
    //    cellClass: "show-cell",
    //    width: "150px",
    //    editable: this.canModify,
    //  },
    //  {
    //    headerName: "稅碼",
    //    field: "taxcode",
    //    enableRowGroup: true,
    //    cellClass: "show-cell",
    //    width: "150px",
    //    editable: this.canModify,
    //  },
    //  {
    //    headerName: "生效日期",
    //    field: "effectiveDate",
    //    enableRowGroup: true,
    //    cellClass: "show-cell",
    //    width: "150px",
    //    editable: this.canModify,
    //    cellEditor: getDatePicker(),
    //  },
    //  {
    //    headerName: "有效日期",
    //    field: "expirationDate",
    //    width: "150px",
    //    editable: this.canModify,
    //    cellEditor: this.getDatePicker(),
    //  }
    //]
  }
  search() {
    this.radioValue = this.matnrList.get('selectedMatnr').value;
    this.rowData_material = [];
    this.rowData_process = [];
    this.rowData_surface = [];
    this.rowData_other = [];
    if (!(this.rfqId) || !(this.radioValue)) {
      return;
    }
    var query={
      rfqId: this.rfqId,
      matnrId: this.radioValue
    };
    this._srmPriceService.GetQotDetail(query).subscribe(result => {
      this.rowData_matnr = [result["matnr"]];
      this.rowData_material = result["material"];
      this.rowData_process = result["process"];
      this.rowData_surface = result["surface"];
      this.rowData_other = result["other"];
      this.rowData_inforecord = result["infoRecord"];
      console.log(result);
    });
  }
  onGridReady_inforecord(params) {
    this.gridApi_inforecord = params.api;
    this.columnApi_inforecord = params.columnApi;
    this.gridApi_inforecord.sizeColumnsToFit();
  }
  start() {
    this.gridApi_inforecord.stopEditing();
    var selectedRows = this.gridApi_inforecord.getSelectedRows();
    if (selectedRows.length == 0) { return;}
    var obj = {
      infos: selectedRows,
      logonid: this._storageService.userName
    };
    this._srmPriceService.Start(obj).subscribe(result => {
      console.log("s");
    });
    console.log(selectedRows);
  }
}

function dateFormatter(data) {
  if (data.value == null) return "";
  var date = new Date(data.value);
  return `${date.getFullYear()}/${date.getMonth() + 1}/${date.getDate()}`;
}
function getDatePicker() {
  function Datepicker() { }
  Datepicker.prototype.init = function (params) {
    this.eInput = document.createElement('input');
    this.eInput.value = params.value;
    this.eInput.classList.add('ag-input');
    this.eInput.style.height = '100%';
    this.eInput.o
    $(this.eInput).datepicker({ dateFormat: 'yy/mm/dd' });
  };
  Datepicker.prototype.getGui = function () {
    return this.eInput;
  };
  Datepicker.prototype.afterGuiAttached = function () {
    this.eInput.focus();
    this.eInput.select();
  };
  Datepicker.prototype.getValue = function () {
    return dateFormatter(this.eInput);
    //return this.eInput.value;
  };
  Datepicker.prototype.destroy = function () { };
  Datepicker.prototype.isPopup = function () {
    return false;
  };
  return Datepicker;
}
