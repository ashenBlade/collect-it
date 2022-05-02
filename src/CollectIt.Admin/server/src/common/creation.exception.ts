export default class CreationException extends Error {
  errors: string[];

  constructor(message: string | null, errors: string[] | null) {
    super(message);
    this.errors = errors ?? [];
  }
}
