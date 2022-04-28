import { NestFactory } from '@nestjs/core';
import { AppModule } from './app.module';

const PORT = 7000;
import * as dotenv from "dotenv";

dotenv.config({ path: `${__dirname}/../development.env` });
async function bootstrap() {
  const app = await NestFactory.create(AppModule, {cors: true});
  app.enableCors({
    origin: 'http://localhost:3000',
    allowedHeaders: '*'
  })
  await app.listen(PORT, () => {
    console.log(`Server started on port ${PORT}`);
  });
}
bootstrap();
