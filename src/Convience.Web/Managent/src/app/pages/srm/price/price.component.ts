import { Component, OnInit, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { SrmRfqService } from '../../../business/srm/srm-rfq.service';
import { SrmPriceService } from '../../../business/srm/srm-price.service';
import { ActivatedRoute } from '@angular/router';
import { Rfq, RfqM } from '../model/rfq';
import { StorageService } from '../../../services/storage.service';
import datepickerFactory from 'jquery-datepicker';
import { NzTreeNodeOptions, NzTreeNode, NzFormatEmitEvent } from 'ng-zorro-antd/tree';
import { ButtonRendererComponent } from './button-cell-renderer.component';
import { SrmModule } from '../srm.module';
import { NzMessageService } from 'ng-zorro-antd/message';

declare const $: any;
datepickerFactory($);
@Component({
  selector: 'app-price',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './price.component.html',
  styleUrls: ['./price.component.less']
})

export class PriceComponent implements OnInit {
  matnrList: FormGroup = new FormGroup({});
  mats: FormGroup = new FormGroup({});
  editForm: FormGroup = new FormGroup({});
  selectedMatnr;
  matnrs = [];
  rfq: Rfq;
  rfqm : RfqM;
  rfqId;
  MaterialList;
  ProcessList;
  SurfaceList;
  OtherList;

  TaxcodeList;
  //taxcode: FormControl;
  CurrencyList;
/*  currency: FormControl;*/
  EkgryList;
  //ekgry: FormControl;


  canModify = true;

  radioValue;

  frameworkComponents: any;

  @ViewChild('ctest1')
  ctest1: TemplateRef<any>;

  @ViewChild('ctest2')
  ctest2: TemplateRef<any>;

  //grid
  columnDefs_matnr
  columnDefs_material;
  columnDefs_process;
  columnDefs_surface;
  columnDefs_other;
  columnDefs_inforecord;
  columnDefs_summary;
  defaultColDef;
  rowSelection = "multiple";
  components;
  isRowSelectable;

  getRowStyle;

  rowData_matnr=[];
  rowData_material=[];
  rowData_process=[];
  rowData_surface=[];
  rowData_other=[];
  rowData_inforecord = [];
  rowData_summary: any;

  gridApi_inforecord;
  columnApi_inforecord;;

  gridApi_summary;
  columnApi_summary;;
  //gird

  nodes: NzTreeNodeOptions[] = [];
  matnrIndex;
  tplModal: NzModalRef;

  //tempqotId;
  //temppriceTotal;
  //tempunit;
  //tempekgry

//  function add(title: TemplateRef<{}>, content: TemplateRef<{}>) {
//  //console.log(this.content);
//  //return;
//  //this._modalService.open(this.content);


//  this.tplModal = this._modalService.create({
//    nzTitle: this.ctest1,
//    nzContent: this.ctest2,
//    nzFooter: null,
//    nzClosable: true,
//    nzMaskClosable: false,
//    nzWidth: 1000,
//  });
//}

  constructor(private activatedRoute: ActivatedRoute
    , private _formBuilder: FormBuilder
    , private _srmRfqService: SrmRfqService
    , private _srmPriceService: SrmPriceService
    , private _storageService: StorageService
    , public _modalService: NzModalService
    ,private _messageService: NzMessageService,  ) {
    this.frameworkComponents = {
      buttonRenderer: ButtonRendererComponent,
    }

    this.getRowStyle = params => {
      if (params.node.rowIndex % 2 === 0) {
        return { background: '#D0D0D0' };
      }
    }

    //this.taxcode = _formBuilder.control([]);
    //this.currency = _formBuilder.control([]);
    //this.ekgry = _formBuilder.control([]);

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


    this.isRowSelectable = function (rowNode) {
      console.log(rowNode.data.rfqNum);
      return rowNode.data.rfqNum ? rowNode.data.isStarted ? false : true : false;
    }

    this.columnDefs_summary = [
      {
        headerName: '操作',
        cellRenderer: 'buttonRenderer',
        cellRendererParams: {
          onClick: this.add.bind(this),
          label: '',
        },
        headerCheckboxSelection: true,
        checkboxSelection: true,
        width: "120px",
        pinned: 'left'
      },
      {
        headerName: "詢價單號",
        field: "rfqNum",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass:"rfq",
        width: "150px",
      },
      {
        headerName: "詢價人員",
        field: "sourcerName",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "rfq",
        width: "150px",
      },
      {
        headerName: "詢價截止日期",
        field: "deadline_str",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "rfq",
        width: "150px",
      },
      {
        headerName: "供應商",
        field: "vendor",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "rfq",
        width: "150px",
      }, {
        headerName: "供應商名稱",
        field: "vendorName",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "rfq",
        width: "150px",
      },
      {
        headerName: "料號",
        field: "matnr",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "rfq",
        width: "150px",
      },
      {
        headerName: "材料規格",
        field: "material",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "rfq",
        width: "150px",
      },
      {
        headerName: "成品尺寸",
        field: "volume",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "rfq",
        width: "150px",
      }, {
        headerName: "工件重量(KG)",
        field: "weight",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "rfq",
        width: "150px",
      }, {
        headerName: "機種名稱",
        field: "machineName",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "rfq",
        width: "150px",
      }, {
        headerName: "報價單號",
        field: "qotNum",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      }, {
        headerName: "材料名稱",
        field: "mMaterial",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "material",
        width: "150px",
      }, {
        headerName: "材料單價(NT/KG)",
        field: "mPrice",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "material",
        width: "150px",
      }, {
        headerName: "長(mm)",
        field: "mLength",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "material",
        width: "150px",
      }, {
        headerName: "寬(mm)",
        field: "mWidth",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "material",
        width: "150px",
      }, {
        headerName: "厚(mm)",
        field: "mHeight",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "material",
        width: "150px",
      },
      {
        headerName: "密度(g/cm3)",
        field: "mDensity",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "material",
        width: "150px",
      },
      {
        headerName: "重量(KG)",
        field: "mWeight",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "material",
        width: "150px",
      },
      {
        headerName: "材料成本(NT)",
        field: "mCostPrice",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "material",
        width: "150px",
      },
      {
        headerName: "備註",
        field: "mNote",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "material",
        width: "150px",
      }, {
        headerName: "機台",
        field: "pMachine",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "process",
        width: "150px",
      }, {
        headerName: "工序",
        field: "pProcessNum",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "process",
        width: "150px",
      }, {
        headerName: "工時(時)",
        field: "pHours",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "process",
        width: "150px",
      }, {
        headerName: "單價(時)",
        field: "pPrice",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "process",
        width: "150px",
      }, {
        headerName: "小計",
        field: "pSubTotal",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "process",
        width: "150px",
      }, {
        headerName: "備註",
        field: "pNote",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "process",
        width: "150px",
      }, {
        headerName: "工序",
        field: "sProcess",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "surface",
        width: "150px",
      }, {
        headerName: "數量",
        field: "sTimes",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "surface",
        width: "150px",
      }, {
        headerName: "單價",
        field: "sPrice",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "surface",
        width: "150px",
      }, {
        headerName: "小計",
        field: "sSubTotal",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "surface",
        width: "150px",
      }, {
        headerName: "計價方式",
        field: "sMethod",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "surface",
        width: "150px",
      }, {
        headerName: "備註",
        field: "sNote",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "surface",
        width: "150px",
      }, {
        headerName: "項目",
        field: "oItem",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "other",
        width: "150px",
      }, {
        headerName: "說明",
        field: "oDescription",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "other",
        width: "150px",
      }, {
        headerName: "單價",
        field: "oPrice",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "other",
        width: "150px",
      }, {
        headerName: "備註",
        field: "oNote",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "other",
        width: "150px",
      },
      {
        headerName: "A(材料)",
        field: "aTotal",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "summary",
        width: "150px",
      },
      {
        headerName: "B(加工費用)",
        field: "bTotal",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "summary",
        width: "150px",
      },
      {
        headerName: "C(表面處理)",
        field: "cTotal",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "summary",
        width: "150px",
      },
      {
        headerName: "D(其他費用)",
        field: "dTotal",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "summary",
        width: "150px",
      },
      {
        headerName: "廠商報價",
        field: "beforePrice",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "summary",
        width: "150px",
      },
      {
        headerName: "議價",
        field: "price",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "summary",
        width: "150px",
      },
      {
        headerName: "價格單位",
        field: "unit",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "summary",
        width: "150px",
      },
      {
        headerName: "幣別",
        field: "currencyName",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "summary",
        width: "150px",
      },
      //{
      //  headerName: "幣別",
      //  field: "currency",
      //  enableRowGroup: true,
      //  cellClass: "show-cell",
      //  width: "150px",
      //},
      {
        headerName: "採購群組",
        field: "ekgry",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "summary",
        width: "150px",
      },
      {
        headerName: "計畫交貨時間",
        field: "leadTime",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "summary",
        width: "150px",
      },
      {
        headerName: "標準採購數量",
        field: "standQty",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "summary",
        width: "150px",
      },
      {
        headerName: "最小採購數量",
        field: "minQty",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "summary",
        width: "150px",
      },
      {
        headerName: "稅碼",
        field: "taxcodeName",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "summary",
        width: "150px",
      },
      //{
      //  headerName: "稅碼",
      //  field: "taxcode",
      //  enableRowGroup: true,
      //  cellClass: "show-cell",
      //  width: "150px",
      //},
      {
        headerName: "生效日期",
        field: "effectiveDate",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "summary",
        width: "150px",
      },
      {
        headerName: "有效日期",
        field: "expirationDate",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "summary",
        width: "150px",
      },
      {
        headerName: "備註",
        field: "note",
        enableRowGroup: true,
        cellClass: "show-cell",
        headerClass: "summary",
        width: "150px",
      },
    ];
  }

  ngOnInit(): void {
    this.matnrList = this._formBuilder.group({
      selectedMatnr: [null]
    });
    this.mats = this._formBuilder.group({});
    this.init();
    this.initGrid();
    this.initTaxCode();
    this.initCurrency();
    this.initEkgry();
  }

  add(e) {
    //this.temppriceTotal = e.rowData.price ? e.rowData.price:"";
    //this.tempunit = e.rowData.unit ? e.rowData.unit:"";
    //console.log(e.rowData.taxcode);
    //this.currency.setValue(e.rowData.currency);
    //this.taxcode.setValue(e.rowData.taxcode);
    //this.ekgry.setValue(e.rowData.ekgry);

    this.editForm = this._formBuilder.group({
      qotId: [null, [Validators.required]],
      //price: ['', [Validators.pattern(/^(0|([1-9](\d)*))(\.(\d)*)?$/)]],
      price: [null, [Validators.required, Validators.pattern(SrmModule.decimalTwoDigits)]],
      unit: [null, [Validators.required, Validators.pattern(SrmModule.number)]],
      currency: [null, [Validators.required]],
      ekgry: [null, [Validators.required]],
      leadTime: [null, [Validators.required, Validators.pattern(SrmModule.decimal)]],
      standQty: [null, [Validators.required, Validators.pattern(SrmModule.decimal)]],
      minQty: [null, [Validators.required, Validators.pattern(SrmModule.decimal)]],
      taxcode: [null, [Validators.required]],
      effectiveDate: [null, [Validators.required]],
      expirationDate: [null, [Validators.required]],
      note: [null, [Validators.required]]
    });


    this.editForm.setValue({
      qotId: e.rowData.qotId
      , price: e.rowData.price ? e.rowData.price : ""
      , unit: e.rowData.unit ? e.rowData.unit : ""
      , currency: e.rowData.currency ? e.rowData.currency : ""
      , ekgry: e.rowData.ekgry ? e.rowData.ekgry : ""
      , leadTime: e.rowData.leadTime ? e.rowData.leadTime : ""
      , standQty: e.rowData.standQty ? e.rowData.standQty : ""
      , minQty: e.rowData.minQty ? e.rowData.minQty : ""
      , taxcode: e.rowData.taxcode ? e.rowData.taxcode : ""
      , effectiveDate: e.rowData.effectiveDate ? e.rowData.effectiveDate : ""
      , expirationDate: e.rowData.expirationDate ? e.rowData.expirationDate : ""
      , note: e.rowData.note ? e.rowData.note : ""
    });
    console.log(e.rowData);

    this.tplModal = this._modalService.create({
    nzTitle: this.ctest1,
    nzContent: this.ctest2,
    nzFooter: null,
    nzClosable: true,
    nzMaskClosable: false,
  });
  }

  edit() {
    //console.log(this.CurrencyList.find(r => r.currency == this.currency.value)?.currencyName);
    //console.log(this.tempqotId);
    console.log(this.editForm);
    for (const i in this.editForm.controls) {
      this.editForm.controls[i].markAsDirty();
      this.editForm.controls[i].updateValueAndValidity();
    }
    if (this.editForm.valid) {
      var r = this.rowData_summary.find(r => r.qotId == this.editForm.get('qotId').value);
      r.price = this.editForm.get('price').value;
      r.unit = this.editForm.get('unit').value;
      r.ekgry = this.editForm.get('ekgry').value;
      r.leadTime = this.editForm.get('leadTime').value;
      r.standQty = this.editForm.get('standQty').value;
      r.minQty = this.editForm.get('minQty').value;
      r.taxcode = this.editForm.get('taxcode').value;//.editForm.get('ekgry').value;
      r.taxcodeName = this.TaxcodeList.find(r => r.taxcode == this.editForm.get('taxcode').value)?.taxcodeName;
      r.currency = this.editForm.get('currency').value;
      r.currencyName = this.CurrencyList.find(r => r.currency == this.editForm.get('currency').value)?.currencyName;
      r.effectiveDate = dateFormatter(this.editForm.get('effectiveDate').value);
      r.expirationDate = dateFormatter(this.editForm.get('expirationDate').value);
      r.note = this.editForm.get('note').value;

      var selectedRows = this.gridApi_summary.getSelectedRows();
      this.gridApi_summary.setRowData(this.rowData_summary);

      this.gridApi_summary.forEachLeafNode((node) => {
        if (selectedRows.find(s => s.qotId == node.data.qotId)){
          node.setSelected(true);
        }
      });
      this.tplModal.close();
    }
  }

  cancelEdit() {
    this.tplModal.close();
  }

  init() {
    this._srmRfqService.GetRfqData(this.rfqId).subscribe(result => {
      this.rfq = result;
      this.matnrs = [];
      console.log(this.rfq);
      this.rfq.m.forEach(row => this.matnrs.push({ label: row.srmMatnr1, value: row.matnrId }));
      this.matnrList.setValue({ selectedMatnr: this.matnrs[0].value });
      this.nodes = [];
      this.nodes = [{ title: this.rfq.h.rfqNum, key: null, icon: 'global', expanded: true, children: [] }];;
      this.rfq.m.forEach((row, index) => this.nodes[0].children.push({ title: row.srmMatnr1, key: row.matnrId.toString(), iicon: 'appstore', children: [], index: index }))
      // { title: department.name, key: department.id, icon: 'appstore', children: [] };
      //this.radioValue = this.matnrs[0].value;
    });
    var query = {
      rfqId: this.rfqId,
    };
    this._srmPriceService.GetSummary(query).subscribe(result => {
      console.log(result);
      this.rowData_summary = result;
    });
  }

  initTaxCode() {
    this._srmPriceService.GetTaxcodes().subscribe(result => {
      console.log(result);
      this.TaxcodeList = result;
    });
  }

  initCurrency() {
    this._srmPriceService.GetCurrency().subscribe(result => {
      console.log(result);
      this.CurrencyList = result;
    });
  }

  initEkgry() {
    console.log(this._storageService.werks);
    this._srmPriceService.GetEkgry(this._storageService.werks.split(',')).subscribe(result => {
      console.log(result);
      this.EkgryList = result;
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
    this.columnDefs_inforecord = [
      {
        headerName: "供應商",
        field: "vendorName",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "A(材料)",
        field: "atotal",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "100px",
      },
      {
        headerName: "B(加工費用)",
        field: "btotal",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "C(表面處理)",
        field: "ctotal",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "D(其他費用)",
        field: "dtotal",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "150px",
      },
      {
        headerName: "廠商報價",
        field: "total",
        enableRowGroup: true,
        cellClass: "show-cell",
        width: "100px",
      },
    ]
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

  changedMatnr(value) {
    console.log(value);
    this.matnrList.setValue({ selectedMatnr: value.keys[0] });
    //if (value.node.origin.index != null) {
      this.matnrIndex = value.node.origin.index;
    //}
    console.log(this.matnrIndex);
    this.search();
  }

  onGridReady_inforecord(params) {
    this.gridApi_inforecord = params.api;
    this.columnApi_inforecord = params.columnApi;
    this.gridApi_inforecord.sizeColumnsToFit();
  }

  onGridReady_summary(params) {
    this.gridApi_summary = params.api;
    this.columnApi_summary = params.columnApi;
  }
  start() {
    //this.gridApi_inforecord.stopEditing();
    //var selectedRows = this.gridApi_inforecord.getSelectedRows();
    var selectedRows = this.gridApi_summary.getSelectedRows();
    if (selectedRows.length == 0) { return;}
    var obj = {
      infos: selectedRows,
      logonid: this._storageService.userName,
      rfqId: this.rfqId
    };
    this._srmPriceService.Start(obj).subscribe(result => {
      console.log("s");
      this._messageService.success("啟動簽核成功！");
    });
    console.log(selectedRows);
  }
}

function dateFormatter(data) {
  if (!data) return "";
  var date = new Date(data);
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
    return dateFormatter(this.eInput.value);
    //return this.eInput.value;
  };
  Datepicker.prototype.destroy = function () { };
  Datepicker.prototype.isPopup = function () {
    return false;
  };
  return Datepicker;
}
