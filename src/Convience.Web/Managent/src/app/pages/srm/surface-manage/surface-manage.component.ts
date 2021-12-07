import { Component, OnInit, TemplateRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { SrmConfigService } from '../../../business/srm/srm-config.service';

@Component({
  selector: 'app-surface-manage',
  templateUrl: './surface-manage.component.html',
  styleUrls: ['./surface-manage.component.less']
})
export class SurfaceManageComponent implements OnInit {
  searchForm: FormGroup = new FormGroup({});
  addForm: FormGroup = new FormGroup({});
  tplModal: NzModalRef;
  data = [];
  page: number = 1;
  size: number = 6;
  total: number;
  _surface;
  constructor(private _srmConfigService: SrmConfigService,
    private _formBuilder: FormBuilder,
    private _modalService: NzModalService,
    private _messageService: NzMessageService,) { }

  ngOnInit(): void {
    this.searchForm = this._formBuilder.group({
      surface: [null]
    });
    this.initaddForm();
    if (sessionStorage.getItem("surface-manage")) {
      var query = JSON.parse(sessionStorage.getItem("surface-manage"));
      this._surface = query.surface;
      this.page = query.page;
      this.size = query.size;
      this.searchForm.patchValue({
        surface: this._surface.SurfaceDesc,
      });
      this.refresh();
    }
  }
  initaddForm() {
    this.addForm = this._formBuilder.group({
      surface: [null, [Validators.required]]
    });
  }
  open(title: TemplateRef<{}>, content: TemplateRef<{}>) {
    this.initaddForm();
    this.tplModal = this._modalService.create({
      nzTitle: title,
      nzContent: content,
      nzFooter: null,
      nzClosable: true,
      nzMaskClosable: false,
    });
  }
  pageChange() {
    this.refresh();
  }
  sizeChange() {
    this.page = 1;
    this.refresh();
  }
  submitSearch() {
    this._surface = {
      SurfaceDesc: this.searchForm.get('surface').value,
    }
    this.page = 1;
    this.refresh();
  }
  refresh() {
    var query = {
      surface: this._surface,
      page: this.page,
      size: this.size
    }
    sessionStorage.setItem("surface-manage", JSON.stringify(query));
    this._srmConfigService.GetSurfaceList(query).subscribe(result => {
      console.log(result);
      this.data = result["data"];
      this.total = result["count"];
    });
  }
  add() {
    for (const i in this.addForm.controls) {
      this.addForm.controls[i].markAsDirty();
      this.addForm.controls[i].updateValueAndValidity();
    }
    if (this.addForm.valid) {
      var surface = {
        SurfaceDesc: this.addForm.get("surface")?.value
      };
      this._srmConfigService.AddSurface(surface).subscribe(result => {
        this._messageService.success("保存成功！");
        this.tplModal.close();
      });
    }
  }
  cancelEdit() {
    this.tplModal.close();
  }
}
