import { NestFactory } from '@nestjs/core';
import { AppModule } from './app.module';

const PORT = 7000;

async function bootstrap() {
  const app = await NestFactory.create(AppModule);

  await app.listen(PORT, () => {
    console.log(`Server started on port ${PORT}`);
  });
}
bootstrap();
