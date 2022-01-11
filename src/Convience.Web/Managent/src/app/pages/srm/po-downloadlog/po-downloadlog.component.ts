import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalService } from 'ng-zorro-antd/modal';
import { SrmPoService } from '../../../business/srm/srm-po.service';
@Component({
  selector: 'app-po-downloadlog',
  templateUrl: './po-downloadlog.component.html',
  styleUrls: ['./po-downloadlog.component.less']
})
export class PoDownloadlogComponent implements OnInit {
  public data: any[] = [];

  private _searchObject: any = {};

  public searchForm: FormGroup = new FormGroup({});

  public settingForm: FormGroup = new FormGroup({});

  // 錶格排序功能

  private _nzModal;

  public page = 1;
  public size = 10;
  public total = 0;

  @ViewChild("settingTpl", { static: true })
  settingTpl;

  constructor(private _modalService: NzModalService,
    private _formBuilder: FormBuilder,
    private _srmPoService: SrmPoService,
    private _messageService: NzMessageService) { }

  ngOnInit(): void {
    this.initSearchForm();
    this.submitSearch();
    //this.initData();
  }

  initData() {
    this._srmPoService.GetDownLoadLog(this._searchObject).subscribe((result: any) => {
      console.info(result);
      this.data = result.data;
      this.total = result.count;
    });
  }
  onChange(result: Date[]): void {
    console.log('onChange: ', result);
  }
  // 初始化搜索錶單
  public initSearchForm() {
    this.searchForm = this._formBuilder.group({
      "sapMatnr": [null],
      "description": [null],
      "username": [null],
      "Date_s": [null],
      "Date_e": [null],
    });
  }

  public pageChange() {
    this.initData();
  }

  public sizeChange() {
    this.page = 1;
    this.initData();
  }

  public submitSearch() {
    this._searchObject = {};
    this._searchObject.sapMatnr = this.searchForm.value["sapMatnr"];
    this._searchObject.description = this.searchForm.value["description"];
    this._searchObject.username = this.searchForm.value["username"];
    this._searchObject.Date_s = this.searchForm.value["Date_s"];
    this._searchObject.Date_e = this.searchForm.value["Date_e"];
    this._searchObject.page = this.page;
    this._searchObject.size = this.size;
    this.initData();
  }

  // public setLoginLogSetting() {
  //   this._loginService.getSettings().subscribe((result: any) => {
  //     this.settingForm = this._formBuilder.group({
  //       "savetime": [result.saveTime, [Validators.required]],
  //     });
  //     this._nzModal = this._modalService.create({
  //       nzContent: this.settingTpl,
  //       nzTitle: "設定登入日誌",
  //       nzFooter: null,
  //     });
  //   });
  // }

  // public submitSetting() {
  //   for (const i in this.settingForm.controls) {
  //     this.settingForm.controls[i].markAsDirty();
  //     this.settingForm.controls[i].updateValueAndValidity();
  //   }
  //   if (this.settingForm.valid) {
  //     this._loginService.updateSetting(this.settingForm.value['savetime']).subscribe(result => {
  //       this._messageService.success("保存成功！");
  //       this._nzModal.close();
  //     });
  //   }
  // }

  public cancelEdit() {
    this._nzModal.close();
  }

}
