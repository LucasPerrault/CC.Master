export enum AttachmentEndReason {
  Modification = 'MODIFICATION',
  Resiliation = 'RESILIATION',
  Deactivation = 'DEACTIVATION',
}

export interface IAttachmentEndReason {
  id: AttachmentEndReason;
  name: string;
}

export const attachmentEndReasons: IAttachmentEndReason[] = [
  {
    id: AttachmentEndReason.Modification,
    name: 'front_contractPage_establishments_endReason_modification',
  },
  {
    id: AttachmentEndReason.Resiliation,
    name: 'front_contractPage_establishments_endReason_resiliation',
  },
  {
    id: AttachmentEndReason.Deactivation,
    name: 'front_contractPage_establishments_endReason_deactivation',
  },
];

export const getAttachmentEndReason = (id: AttachmentEndReason): IAttachmentEndReason =>
  attachmentEndReasons.find(r => r.id === id);
