import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NzDatePickerComponent} from 'ng-zorro-antd/date-picker';
import datepickerFactory from 'jquery-datepicker';
import datepickerJAFactory from 'jquery-datepicker/i18n/jquery.ui.datepicker-en-GB';
import { AgGridDatePickerComponent} from './AGGridDatePickerCompponent';
declare const $: any; // avoid the error on $(this.eInput).datepicker();
datepickerFactory($);
datepickerJAFactory($);
@Component({
  selector: 'app-po',
  templateUrl: './po.component.html',
  styleUrls: ['./po.component.less']
})
export class PoComponent implements OnInit {
  gridApi;
  gridColumnApi;
  columnDefs;
  defaultColDef;
  detailCellRendererParams;
  components;
  rowData: any;
  constructor(private http: HttpClient) {
    this.columnDefs = [
      {
        headerName:'採購單識別碼',
        field: 'PoId',
        cellRenderer: 'agGroupCellRenderer',
      },
      {
        headerName:'採購單號',
        field: 'PoNum',
      },
      {
        headerName:'供應商識別碼',
        field: 'VendorId',
        hide:'true',
      },
      {
        headerName:'供應商',
        field: 'vendor_name',
      },
      {
        headerName:'狀態',
        field: 'Status',
      },
      {
        headerName:'採購單總金額',
        field: 'TotalAmount',
      },
      {
        headerName:'採購人員',
        field: 'Buyer',
      },
      {
        headerName:'採購組織',
        field: 'Org',
      },
      {
        headerName:'文件日期',
        field: 'DocDate',
      },
      {
        headerName:'廠商接收日期',
        field: 'ReplyDate',
      },
      {
        headerName:'拋轉日期',
        field: 'CreateDate',
      },
    //   { field: 'calls' },
    //   {
    //     field: 'minutes',
    //     valueFormatter: "x.toLocaleString() + 'm'",
    //   },
    //   {
    //     field: 'date',
    //     editable: true,
    //     // valueFormatter: this.expiryDateFormatter,
    // cellEditorFramework: AgGridDatePickerComponent
    //   },
    ];
    this.defaultColDef = { flex: 1 };
    this.components = { datePicker: getDatePicker() };
    this.detailCellRendererParams = {
      detailGridOptions: {
        columnDefs: [
          {
            headerName:'採購單明細識別碼',
            field: 'PoLId',
          },
          {
            PoId:'採購單識別碼',
            field: 'PoId',
          },
          {
            headerName:'料號識別碼',
            field: 'MantrId',
          },
          {
            headerName:'物料內文',
            field: 'Description',
          },
          {
            headerName:'數量',
            field: 'Qty',
          },
          {
            headerName:'單價',
            field: 'Price',
          },
          {
            headerName:'交貨日期',
            field: 'DeliveryDate',
          },
          {
            headerName:'廠商交貨日期',
            field: 'ReplyDeliveryDate',
          },
          {
            headerName:'交貨地點',
            field: 'DeliveryPlace',
          },
          {
            headerName:'關鍵零組件/首件',
            field: 'CriticalPart',
          },
          {
            headerName:'檢驗時間',
            field: 'InspectionTime',
          },
          {
            field: 'date',
            editable: true,
            // valueFormatter: this.expiryDateFormatter,
        cellEditorFramework: AgGridDatePickerComponent
          },
        ],
        defaultColDef: { flex: 1 },
      },

      getDetailRowData: function (params) {
        params.successCallback(params.data.SrmPoLs);

      },
    };
    console.info(this.detailCellRendererParams);
  }

  ngOnInit(): void {
  }
  expiryDateFormatter(params) {
    if (params.value) {
      return `${params.value.date.month - 1}/${params.value.date.year}`;
     }
  }
  onFirstDataRendered(params) {
    // setTimeout(function () {
    //   params.api.getDisplayedRowAtIndex(1).setExpanded(true);
    // }, 0);
  }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;

    this.http
      .get('http://localhost:5000/api/SrmPo/GetPo')
      .subscribe((data) => {
        this.rowData = data;
        console.info(data);
      });
  }

}

function getDatePicker() {
  function Datepicker() {}
  Datepicker.prototype.init = function (params) {
    this.eInput = document.createElement('input');
    this.eInput.value = params.value;
    this.eInput.classList.add('ag-input');
    this.eInput.style.height = '100%';
    $(this.eInput).datepicker({ dateFormat: 'dd/mm/yy' });
  };
  Datepicker.prototype.getGui = function () {
    return this.eInput;
  };
  Datepicker.prototype.afterGuiAttached = function () {
    this.eInput.focus();
    this.eInput.select();
  };
  Datepicker.prototype.getValue = function () {
    return this.eInput.value;
  };
  Datepicker.prototype.destroy = function () {};
  Datepicker.prototype.isPopup = function () {
    return false;
  };
  return Datepicker;
}
