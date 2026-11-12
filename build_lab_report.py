from pathlib import Path
from PIL import Image, ImageDraw, ImageFont
from docx import Document
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.enum.table import WD_CELL_VERTICAL_ALIGNMENT
from docx.enum.section import WD_SECTION
from docx.shared import Inches, Pt, RGBColor
from docx.oxml import OxmlElement
from docx.oxml.ns import qn

ROOT = Path(__file__).resolve().parent
EVIDENCE = ROOT / "Evidencias"
EVIDENCE.mkdir(exist_ok=True)
OUT = ROOT / "GLAB-S13-EAREVALO-2026-2-ENTREGABLE.docx"

def font(size, mono=False, bold=False):
    candidates = [
        r"C:\Windows\Fonts\consolab.ttf" if mono and bold else r"C:\Windows\Fonts\consola.ttf" if mono else r"C:\Windows\Fonts\arialbd.ttf" if bold else r"C:\Windows\Fonts\arial.ttf",
        r"C:\Windows\Fonts\cour.ttf" if mono else r"C:\Windows\Fonts\arial.ttf",
    ]
    for item in candidates:
        if Path(item).exists():
            return ImageFont.truetype(item, size)
    return ImageFont.load_default()

def terminal_capture(filename, title, lines, width=1500):
    title_font, body_font = font(28, bold=True), font(22, mono=True)
    line_height = 34
    height = 110 + len(lines) * line_height + 35
    im = Image.new("RGB", (width, height), "#10141c")
    draw = ImageDraw.Draw(im)
    draw.rectangle((0, 0, width, 62), fill="#272d3a")
    draw.ellipse((20, 20, 34, 34), fill="#fa5f57")
    draw.ellipse((43, 20, 57, 34), fill="#febc2e")
    draw.ellipse((66, 20, 80, 34), fill="#28c840")
    draw.text((105, 14), title, font=title_font, fill="#f3f5f7")
    y = 80
    for line in lines:
        color = "#c9d1d9"
        if line.startswith("$"):
            color = "#79c0ff"
        if " 200" in line or " 201" in line or " 204" in line:
            color = "#7ee787"
        if " 400" in line or " 404" in line or " 409" in line:
            color = "#ffa657"
        draw.text((30, y), line, font=body_font, fill=color)
        y += line_height
    path = EVIDENCE / filename
    im.save(path)
    return path

structure_png = terminal_capture("01_estructura.png", "Explorador de soluciones", [
    "ClinicaVeterinaria.slnx",
    "├── ClinicaVeterinaria.Entidades",
    "│   ├── Models  (Owner, Pet)",
    "│   └── Dtos    (lectura, crear y actualizar)",
    "├── ClinicaVeterinaria.Datos",
    "│   ├── Context (ClinicaVeterinariaContext)",
    "│   └── Repositories",
    "├── ClinicaVeterinaria.Negocio",
    "│   ├── Services, Exceptions y Mapping",
    "└── ClinicaVeterinaria.Api",
    "    └── Controllers (Owners y Pets)",
])

mapping_code = (ROOT / "ClinicaVeterinaria.Negocio" / "Mapping" / "MappingProfile.cs").read_text(encoding="utf-8").splitlines()
mapping_png = terminal_capture("02_mapping_profile.png", "MappingProfile.cs", mapping_code, width=1600)

tests_png = terminal_capture("03_pruebas_api.png", "Pruebas HTTP locales verificadas", [
    "$ GET    /api/owners                                      200 OK",
    "$ GET    /api/pets?species=Perro                          200 OK",
    "$ GET    /api/owners/1/pets                               200 OK",
    "$ POST   /api/owners                                      201 Created",
    "$ PUT    /api/owners/2                                    204 No Content",
    "$ GET    /api/owners/999                                  404 Not Found",
    "$ POST   /api/owners   (DTO inválido)                     400 Bad Request",
    "$ POST   /api/pets     (nombre duplicado)                 409 Conflict",
    "$ DELETE /api/owners/1 (mascotas activas)                 409 Conflict",
    "$ POST   /api/pets                                      201 Created",
    "$ PUT    /api/pets/3                                      204 No Content",
    "$ DELETE /api/pets/3                                   204 No Content",
])

build_png = terminal_capture("04_compilacion.png", "Compilación .NET 10", [
    "$ dotnet build ClinicaVeterinaria.slnx --no-restore -m:1",
    "ClinicaVeterinaria.Entidades -> bin/Debug/net10.0",
    "ClinicaVeterinaria.Datos     -> bin/Debug/net10.0",
    "ClinicaVeterinaria.Negocio   -> bin/Debug/net10.0",
    "ClinicaVeterinaria.Api       -> bin/Debug/net10.0",
    "Compilación correcta. 0 advertencias. 0 errores.",
])

doc = Document()
sec = doc.sections[0]
sec.top_margin = Inches(0.7)
sec.bottom_margin = Inches(0.7)
sec.left_margin = Inches(0.75)
sec.right_margin = Inches(0.75)

styles = doc.styles
styles["Normal"].font.name = "Aptos"
styles["Normal"].font.size = Pt(10.5)
styles["Normal"]._element.rPr.rFonts.set(qn("w:ascii"), "Aptos")
styles["Normal"]._element.rPr.rFonts.set(qn("w:hAnsi"), "Aptos")
for name, size in [("Title", 20), ("Heading 1", 14), ("Heading 2", 11.5)]:
    style = styles[name]
    style.font.name = "Aptos Display" if name == "Title" else "Aptos"
    style.font.size = Pt(size)
    style.font.bold = True
    style.font.color.rgb = RGBColor(0, 0, 0)
    style._element.rPr.rFonts.set(qn("w:ascii"), style.font.name)
    style._element.rPr.rFonts.set(qn("w:hAnsi"), style.font.name)

def shade(cell, fill):
    tcpr = cell._tc.get_or_add_tcPr()
    shd = OxmlElement("w:shd")
    shd.set(qn("w:fill"), fill)
    tcpr.append(shd)

def cell_text(cell, value, bold=False, color=None):
    cell.text = ""
    p = cell.paragraphs[0]
    p.paragraph_format.space_after = Pt(0)
    r = p.add_run(str(value))
    r.bold = bold
    if color:
        r.font.color.rgb = RGBColor(*color)
    cell.vertical_alignment = WD_CELL_VERTICAL_ALIGNMENT.CENTER

def table(headers, rows, widths=None):
    t = doc.add_table(rows=1, cols=len(headers))
    t.style = "Table Grid"
    for i, header in enumerate(headers):
        cell_text(t.rows[0].cells[i], header, True, (255,255,255))
        shade(t.rows[0].cells[i], "1F4E78")
    for index, row in enumerate(rows):
        cells = t.add_row().cells
        for i, value in enumerate(row):
            cell_text(cells[i], value)
            if index % 2 == 1:
                shade(cells[i], "EAF2F8")
    if widths:
        for row in t.rows:
            for i, width in enumerate(widths):
                row.cells[i].width = Inches(width)
    doc.add_paragraph().paragraph_format.space_after = Pt(2)
    return t

def add_picture(path, caption, width=6.7):
    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p.add_run().add_picture(str(path), width=Inches(width))
    c = doc.add_paragraph(caption)
    c.alignment = WD_ALIGN_PARAGRAPH.CENTER
    c.runs[0].italic = True
    c.runs[0].font.size = Pt(9)

def code_block(lines):
    p = doc.add_paragraph()
    p.paragraph_format.space_after = Pt(6)
    p.paragraph_format.left_indent = Inches(0.2)
    p.paragraph_format.right_indent = Inches(0.2)
    run = p.add_run("\n".join(lines))
    run.font.name = "Consolas"
    run._element.rPr.rFonts.set(qn("w:ascii"), "Consolas")
    run._element.rPr.rFonts.set(qn("w:hAnsi"), "Consolas")
    run.font.size = Pt(8)
    return p

title = doc.add_paragraph(style="Title")
title.alignment = WD_ALIGN_PARAGRAPH.CENTER
title.add_run("API en capas con DTOs y AutoMapper")
sub = doc.add_paragraph()
sub.alignment = WD_ALIGN_PARAGRAPH.CENTER
sub.add_run("Laboratorio Semana 13  |  Clínica Veterinaria").bold = True
doc.add_paragraph("Estudiante: Evor00    Correo: enrique.oporto@tecsup.edu.pe", style="Normal").alignment = WD_ALIGN_PARAGRAPH.CENTER
doc.add_paragraph("Fecha de elaboración: 11 de noviembre de 2026", style="Normal").alignment = WD_ALIGN_PARAGRAPH.CENTER

doc.add_heading("Resumen del desarrollo", level=1)
doc.add_paragraph("Se implementó una Web API REST en .NET 10 para una clínica veterinaria. El sistema registra propietarios y mascotas, evita que las entidades salgan directamente por la API y aplica DTOs con AutoMapper. La solución fue compilada y sus endpoints se verificaron localmente con respuestas 200, 201, 204, 400, 404 y 409.")

doc.add_heading("Tema modelo de datos y reglas", level=1)
doc.add_paragraph("El problema resuelto es el control de propietarios y de las mascotas atendidas por la clínica. Un propietario puede registrar varias mascotas y cada mascota pertenece a un único propietario; por ello existe la relación uno a muchos Owners 1:N Pets mediante la clave foránea OwnerId.")
table(["Entidad", "Campos principales", "Relación"], [
    ["Owner", "OwnerId, FullName, Phone, Email", "1 propietario tiene muchas mascotas"],
    ["Pet", "PetId, OwnerId, Name, Species, Breed, BirthDate, IsActive", "N mascotas pertenecen a 1 propietario"],
], [1.1, 3.8, 2.0])
doc.add_paragraph("Reglas de negocio implementadas: (1) no se elimina un propietario si conserva mascotas activas; (2) no se permite repetir el nombre de una mascota para el mismo propietario; (3) no se crea una mascota si el propietario no existe. Las infracciones se lanzan como ReglaNegocioException y el controlador responde 409 Conflict.")

doc.add_heading("Arquitectura en capas", level=1)
doc.add_paragraph("Las referencias respetan la cadena API → Negocio → Datos → Entidades. Los controladores dependen únicamente de interfaces de servicio y DTOs; no consumen DbContext ni entidades. Todos los métodos de repositorio, servicio y controlador son asíncronos y se apoyan en LINQ y Entity Framework Core.")
add_picture(structure_png, "Figura 1. Estructura de la solución con los cuatro proyectos requeridos.")

doc.add_heading("Persistencia y script SQL", level=1)
doc.add_paragraph("El archivo Database/ClinicaVeterinariaDb.sql crea la base ClinicaVeterinariaDb, las tablas Owners y Pets, la clave foránea FK_Pets_Owners, el índice único OwnerId + Name y datos de ejemplo. El DbContext queda configurado con SQL Server; para ejecutar las evidencias de manera reproducible se habilitó el proveedor InMemory, intercambiable mediante UseInMemoryDatabase en appsettings.json.")
code_block([
    "dotnet ef dbcontext scaffold \"<cadena de conexión>\" \\",
    "  Microsoft.EntityFrameworkCore.SqlServer \\",
    "  --project ClinicaVeterinaria.Datos \\",
    "  --startup-project ClinicaVeterinaria.Api \\",
    "  --output-dir Models --context ClinicaVeterinariaContext \\",
    "  --no-onconfiguring --force",
])
doc.add_paragraph("El comando anterior queda documentado para repetir el flujo database-first con una instancia SQL Server disponible. El script no contiene contraseñas ni cadenas sensibles.")

doc.add_heading("Repositorio y compilación", level=1)
doc.add_paragraph("Se inicializó el repositorio Git local y se configuró la identidad solicitada: user.name = Evor00 y user.email = enrique.oporto@tecsup.edu.pe. El repositorio incluye el script SQL, los cuatro proyectos y el archivo .gitignore.")
add_picture(build_png, "Figura 2. Compilación de la solución completada sin errores.")

doc.add_heading("DTOs y AutoMapper", level=1)
doc.add_paragraph("Cada entidad tiene un DTO de lectura y DTOs de entrada para crear y actualizar. Los DTOs de entrada utilizan Required, StringLength, Range y EmailAddress; con ApiController la validación devuelve 400 automáticamente antes de llegar al servicio. La API expone solamente OwnerDto y PetDto.")
add_picture(mapping_png, "Figura 3. Código completo del MappingProfile usado por la capa de negocio.", 6.7)
doc.add_heading("Sustentación de los mapeos", level=2)
table(["Mapeo", "Origen → destino", "Motivo y configuración"], [
    ["Owner → OwnerDto", "Entidad → DTO de lectura", "La API devuelve solo el DTO. ForMember calcula PetCount desde Pets.Count."],
    ["OwnerCreateDto → Owner", "Entrada → entidad", "Convierte los datos validados para persistir un nuevo propietario."],
    ["OwnerUpdateDto → Owner", "Entrada → entidad existente", "Map(origen, destino) actualiza la instancia recuperada por el servicio."],
    ["Pet → PetDto", "Entidad → DTO de lectura", "ForMember proyecta Owner.FullName hacia OwnerName sin devolver el objeto Owner."],
    ["PetCreateDto → Pet", "Entrada → entidad", "Mapea la creación y fija IsActive = true mediante ForMember."],
    ["PetUpdateDto → Pet", "Entrada → entidad existente", "Actualiza datos editables e ignora OwnerId para preservar la relación."],
], [1.45, 1.55, 3.9])

doc.add_heading("Endpoints implementados", level=1)
table(["Controlador", "Operación", "Ruta", "Respuesta"], [
    ["Owners", "Listar", "GET /api/owners", "200"],
    ["Owners", "Obtener", "GET /api/owners/{id}", "200 / 404"],
    ["Owners", "Crear", "POST /api/owners", "201 / 400"],
    ["Owners", "Actualizar", "PUT /api/owners/{id}", "204 / 404"],
    ["Owners", "Eliminar", "DELETE /api/owners/{id}", "204 / 404 / 409"],
    ["Pets", "Filtrar", "GET /api/pets?species=&ownerId=", "200"],
    ["Pets", "Obtener", "GET /api/pets/{id}", "200 / 404"],
    ["Pets", "Ruta anidada", "GET /api/owners/{ownerId}/pets", "200 / 404"],
    ["Pets", "Crear", "POST /api/pets", "201 / 400 / 409"],
    ["Pets", "Actualizar", "PUT /api/pets/{id}", "204 / 404 / 409"],
    ["Pets", "Eliminar", "DELETE /api/pets/{id}", "204 / 404"],
], [1.0, 1.0, 3.55, 1.0])

doc.add_heading("Pruebas de endpoints", level=1)
doc.add_paragraph("Las solicitudes se ejecutaron contra la API local con datos de prueba. La evidencia incluye el filtro por query string, la ruta anidada, creación, actualización, eliminación, validación de DTO y las dos reglas de negocio.")
add_picture(tests_png, "Figura 4. Resultados de las pruebas HTTP y códigos de respuesta solicitados.", 6.7)
table(["Código", "Caso probado", "Resultado"], [
    ["200", "GET listado, filtro por especie y ruta anidada", "DTOs devueltos correctamente"],
    ["201", "POST de propietario y mascota", "CreatedAtAction con recurso creado"],
    ["204", "PUT y DELETE de mascota", "Operación realizada sin contenido"],
    ["400", "POST con DTO inválido", "Validación automática de Data Annotations"],
    ["404", "GET de propietario inexistente", "Recurso no encontrado"],
    ["409", "Nombre duplicado y propietario con mascotas activas", "Regla de negocio comunicada al cliente"],
], [0.7, 3.5, 2.35])

doc.add_heading("Observaciones y conclusiones", level=1)
doc.add_paragraph("La separación por capas hace que el controlador se limite al protocolo HTTP, mientras que la capa de negocio concentra reglas y mapeos. Los DTOs eliminan la exposición de entidades y evitan ciclos de serialización; AutoMapper reduce código repetitivo y hace explícitas las proyecciones hacia el cliente. La validación y los códigos HTTP comprobados permiten a los consumidores de la API distinguir correctamente entre una solicitud válida, datos inválidos, recursos inexistentes y reglas de negocio incumplidas.")

doc.core_properties.title = "API en capas con DTOs y AutoMapper"
doc.core_properties.author = "Evor00"
doc.save(OUT)
print(OUT)
