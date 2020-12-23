export enum ToastType {
    Error,
    Success,
    Warning,
}

export interface IToast {
    id?: string;
    type: ToastType;
    message: string;
    duration?: number;
}
