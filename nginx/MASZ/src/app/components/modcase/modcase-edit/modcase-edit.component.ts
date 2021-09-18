import { ENTER, COMMA, SPACE } from '@angular/cdk/keycodes';
import { HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatChipInputEvent } from '@angular/material/chips';
import { MatDialog } from '@angular/material/dialog';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { startWith, map } from 'rxjs/operators';
import { APIEnumTypes } from 'src/app/models/APIEmumTypes';
import { APIEnum } from 'src/app/models/APIEnum';
import { ContentLoading } from 'src/app/models/ContentLoading';
import { DiscordUser } from 'src/app/models/DiscordUser';
import { ModCase } from 'src/app/models/ModCase';
import { PunishmentType } from 'src/app/models/PunishmentType';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import { EnumManagerService } from 'src/app/services/enum-manager.service';

@Component({
  selector: 'app-modcase-edit',
  templateUrl: './modcase-edit.component.html',
  styleUrls: ['./modcase-edit.component.css']
})
export class ModcaseEditComponent implements OnInit {

  public memberFormGroup!: FormGroup;
  public infoFormGroup!: FormGroup;
  public punishmentFormGroup!: FormGroup;
  public filesFormGroup!: FormGroup;
  public optionsFormGroup!: FormGroup;

  readonly separatorKeysCodes: number[] = [ENTER, COMMA, SPACE];
  public caseLabels: string[] = [];

  public savingCase: boolean = false;

  public memberForm = new FormControl();
  public filteredMembers!: Observable<DiscordUser[]>;

  public guildId!: string;
  public caseId!: string;
  public members: ContentLoading<DiscordUser[]> = { loading: true, content: [] };
  public oldCase: ContentLoading<ModCase> = { loading: true, content: undefined };
  public punishmentOptions: ContentLoading<APIEnum[]> = { loading: true, content: [] };

  constructor(private _formBuilder: FormBuilder, private api: ApiService, private toastr: ToastrService, private authService: AuthService, private router: Router, private route: ActivatedRoute, private dialog: MatDialog, private enumManager: EnumManagerService) { }

  ngOnInit(): void {
    this.guildId = this.route.snapshot.paramMap.get('guildid') as string;
    this.caseId = this.route.snapshot.paramMap.get('caseid') as string;

    this.memberFormGroup = this._formBuilder.group({
      member: ['', Validators.required]
    });
    this.infoFormGroup = this._formBuilder.group({
      title: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', Validators.required]
    });
    this.punishmentFormGroup = this._formBuilder.group({
      punishmentType: ['', Validators.required],
      dmNotification: [''],
      handlePunishment: [''],
      punishedUntil: ['']
    });
    this.optionsFormGroup = this._formBuilder.group({
      sendNotification: ['']
    });

    this.optionsFormGroup.controls['sendNotification'].setValue(true);

    this.punishmentFormGroup.get('punishmentType')?.valueChanges.subscribe((val: PunishmentType) => {
      if (val !== PunishmentType.Ban && val !== PunishmentType.Mute) {
        this.punishmentFormGroup.controls['punishedUntil'].setValue(null);
        this.punishmentFormGroup.controls['punishedUntil'].updateValueAndValidity();
      }
      if (val === PunishmentType.None) {
        this.punishmentFormGroup.controls['handlePunishment'].setValue(false);
      } else {
        this.punishmentFormGroup.controls['handlePunishment'].setValue(true);
      }
    });

    this.filteredMembers = this.memberFormGroup.valueChanges
      .pipe(
        startWith(''),
        map(value => this._filter(value.member))
      );
    this.reload();
  }

  reload() {
    this.members = { loading: true, content: [] };
    this.oldCase = { loading: true, content: undefined };

    this.api.getSimpleData(`/guilds/${this.guildId}/cases/${this.caseId}`).subscribe((data) => {
      this.oldCase.content = data;
      this.applyOldCase(data);
      this.oldCase.loading = false;
    }, () => {
      this.toastr.error("Failed to load current case.");
      this.oldCase.loading = false;
    });

    this.enumManager.getEnum(APIEnumTypes.PUNISHMENT).subscribe((data) => {
      this.punishmentOptions.content = data;
      this.punishmentOptions.loading = false;
    }, () => {
      this.punishmentOptions.loading = false;
      this.toastr.error("Failed to load punishment enum.");
    });

    let params = new HttpParams()
          .set('partial', 'true');
    this.api.getSimpleData(`/discord/guilds/${this.guildId}/members`, true, params).subscribe((data) => {
      this.members.content = data;
      this.members.loading = false;
    }, () => {
      this.toastr.error("Failed to load member list.");
    });
  }

  private _filter(value: string): DiscordUser[] {
    if (!value?.trim()) {
      return this.members.content?.filter(option => !option.bot)?.slice(0, 10) as DiscordUser[];
    }
    const filterValue = value.trim().toLowerCase();

    return this.members.content?.filter(option =>
       ((option.username + "#" + option.discriminator).toLowerCase().includes(filterValue) ||
       option.id.includes(filterValue)) && !option.bot).slice(0, 10) as DiscordUser[];
  }

  public applyOldCase(modCase: ModCase) {
    this.memberFormGroup.setValue({
      member: modCase.userId
    });
    this.infoFormGroup.setValue({
      title: modCase.title,
      description: modCase.description
    });
    this.caseLabels = modCase.labels;
    this.punishmentFormGroup.setValue({
      punishmentType: modCase.punishmentType,
      dmNotification: false,
      handlePunishment: false,
      punishedUntil: modCase.punishedUntil
    });
    this.optionsFormGroup.setValue({
      sendNotification: false
    });
  }

  public updateCase() {
    this.savingCase = true;
    const data = {
      title: this.infoFormGroup.value.title,
      description: this.infoFormGroup.value.description,
      userid: this.memberFormGroup.value.member?.trim(),
      labels: this.caseLabels,
      punishmentType: this.punishmentFormGroup.value.punishmentType,
      punishedUntil: (typeof this.punishmentFormGroup.value.punishedUntil === 'string') ? this.punishmentFormGroup.value.punishedUntil : this.punishmentFormGroup.value.punishedUntil?.toISOString() ?? null,
    };
    const params = new HttpParams()
      .set('sendnotification', this.optionsFormGroup.value.sendNotification ? 'true' : 'false')
      .set('handlePunishment', this.punishmentFormGroup.value.handlePunishment ? 'true' : 'false')
      .set('sendDmNotification', this.punishmentFormGroup.value.dmNotification ? 'true' : 'false');

      this.api.putSimpleData(`/guilds/${this.guildId}/cases/${this.caseId}`, data, params, true, true).subscribe((data) => {
        const caseId = data.caseId;
        this.router.navigate(['guilds', this.guildId, 'cases', caseId]);
        this.savingCase = false;
        this.toastr.success(`Case ${caseId} updated.`);
      }, () => { this.savingCase = false; });
  }

  add(event: MatChipInputEvent): void {
    const input = event.input;
    const value = event.value;

    if ((value || '').trim()) {
      this.caseLabels.push(value.trim());
    }

    if (input) {
      input.value = '';
    }
  }

  remove(label: string): void {
    const index = this.caseLabels.indexOf(label);

    if (index >= 0) {
      this.caseLabels.splice(index, 1);
    }
  }
}
