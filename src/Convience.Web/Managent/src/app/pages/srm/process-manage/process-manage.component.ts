import { Component, OnInit, TemplateRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { SrmConfigService } from '../../../business/srm/srm-config.service';

@Component({
  selector: 'app-process-manage',
  templateUrl: './process-manage.component.html',
  styleUrls: ['./process-manage.component.less']
})
export class ProcessManageComponent implements OnInit {
  searchForm: FormGroup = new FormGroup({});
  addForm: FormGroup = new FormGroup({});
  tplModal: NzModalRef;
  data = [];
  page: number = 1;
  size: number = 6;
  total: number;
  _process;
  constructor(private _srmConfigService: SrmConfigService,
    private _formBuilder: FormBuilder,
    private _modalService: NzModalService,
    private _messageService: NzMessageService,) { }

  ngOnInit(): void {
    this.searchForm = this._formBuilder.group({
      process: [null]
    });
    this.initaddForm();
    if (sessionStorage.getItem("process-manage")) {
      var query = JSON.parse(sessionStorage.getItem("process-manage"));
      this._process = query.process;
      this.page = query.page;
      this.size = query.size;
      this.searchForm.patchValue({
        process: this._process.Process,
      });
      this.refresh();
    }
  }
  initaddForm() {
    this.addForm = this._formBuilder.group({
      process: [null, [Validators.required]]
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
    this._process = {
      Process: this.searchForm.get('process').value,
    }
    this.page = 1;
    this.refresh();
  }
  refresh() {
    var query = {
      process: this._process,
      page: this.page,
      size: this.size
    }
    sessionStorage.setItem("process-manage", JSON.stringify(query));
    this._srmConfigService.GetProcessList(query).subscribe(result => {
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
      var process = {
        Process: this.addForm.get("process")?.value
      };
      this._srmConfigService.AddProcess(process).subscribe(result => {
        this._messageService.success("保存成功！");
        this.tplModal.close();
      });
    }
  }
  cancelEdit() {
    this.tplModal.close();
  }
}
