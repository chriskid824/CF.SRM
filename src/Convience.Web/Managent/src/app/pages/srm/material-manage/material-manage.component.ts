import { Component, OnInit, TemplateRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { SrmMaterialTrendService } from '../../../business/srm/srm-materialTrend.service';

@Component({
  selector: 'app-material-manage',
  templateUrl: './material-manage.component.html',
  styleUrls: ['./material-manage.component.less']
})
export class MaterialManageComponent implements OnInit {
  searchForm: FormGroup = new FormGroup({});
  addForm: FormGroup = new FormGroup({});
  tplModal: NzModalRef;
  data = [];
  page: number = 1;
  size: number = 6;
  total: number;
  _material;

  constructor(private _srmMaterialTrendService: SrmMaterialTrendService,
    private _formBuilder: FormBuilder,
    private _modalService: NzModalService,
    private _messageService: NzMessageService,  ) { }

  ngOnInit(): void {
    this.searchForm = this._formBuilder.group({
      material: [null]
    });
    this.initaddForm();
  }
  initaddForm() {
    this.addForm = this._formBuilder.group({
      material: [null, [Validators.required]]
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
    this._material = {
      Material: this.searchForm.get('material').value,
    }
    this.page = 1;
    this.refresh();
  }
  refresh() {
    var query = {
      material: this._material,
      page: this.page,
      size: this.size
    }
    sessionStorage.setItem("material-manage", JSON.stringify(query));
    this._srmMaterialTrendService.GetMaterialList(query).subscribe(result => {
      console.log(result);
      this.data = result["data"];
      this.total = result["count"];
    });
  }
  add() {
    var material = {
      Material: this.addForm.get("material")?.value
    };
    this._srmMaterialTrendService.AddMaterial(material).subscribe(result => {
      this._messageService.success("保存成功！");
      this.tplModal.close();
    });
  }
  cancelEdit() {
    this.tplModal.close();
  }
}
