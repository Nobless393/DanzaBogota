import { useEffect, useState, type FormEvent } from "react";
import { useNavigate, useParams, Link } from "react-router-dom";
import { eventsApi } from "../../api/events";
import { catalogApi } from "../../api/catalog";
import { extractErrorMessage } from "../../api/client";
import type { CategoryDto, EventTypeDto, LocalityDto } from "../../types";
import { Alert, Button, Card, Input, PageLoader, Select, Textarea } from "../../components/ui";
import { toLocalDateTimeInput } from "../../utils/format";

interface FormState {
  title: string;
  description: string;
  imageUrl: string;
  startDate: string;
  endDate: string;
  locationName: string;
  address: string;
  localityId: string;
  maxParticipants: string;
  maxParticipantsPerGroup: string;
  eventTypeIds: string[];
  categoryIds: string[];
}

const emptyForm: FormState = {
  title: "",
  description: "",
  imageUrl: "",
  startDate: "",
  endDate: "",
  locationName: "",
  address: "",
  localityId: "",
  maxParticipants: "",
  maxParticipantsPerGroup: "",
  eventTypeIds: [],
  categoryIds: [],
};

export default function EventForm() {
  const { id } = useParams<{ id: string }>();
  const isEdit = Boolean(id);
  const navigate = useNavigate();

  const [form, setForm] = useState<FormState>(emptyForm);
  const [localities, setLocalities] = useState<LocalityDto[]>([]);
  const [eventTypes, setEventTypes] = useState<EventTypeDto[]>([]);
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [loading, setLoading] = useState(isEdit);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [isPast, setIsPast] = useState(false);

  useEffect(() => {
    let mounted = true;
    const loadCatalogs = async () => {
      try {
        const [locs, types, cats] = await Promise.all([
          catalogApi.getLocalities(),
          catalogApi.getEventTypes(),
          catalogApi.getCategories(),
        ]);
        if (!mounted) return;
        setLocalities(locs);
        setEventTypes(types);
        setCategories(cats);
      } catch (err) {
        if (mounted) setError(extractErrorMessage(err));
      }
    };
    loadCatalogs();

    if (isEdit) {
      eventsApi
        .getById(Number(id))
        .then((event) => {
          if (!mounted) return;
          setIsPast(new Date(event.endDate) < new Date());
          setForm({
            title: event.title,
            description: event.description,
            imageUrl: event.imageUrl ?? "",
            startDate: toLocalDateTimeInput(event.startDate),
            endDate: toLocalDateTimeInput(event.endDate),
            locationName: event.locationName,
            address: event.address,
            localityId: String(event.localityId),
            maxParticipants: String(event.maxParticipants),
            maxParticipantsPerGroup: String(event.maxParticipantsPerGroup),
            eventTypeIds: (event.eventTypeIds ?? []).map(String),
            categoryIds: (event.categoryIds ?? []).map(String),
          });
        })
        .catch((err) => mounted && setError(extractErrorMessage(err)))
        .finally(() => mounted && setLoading(false));
    }
    return () => {
      mounted = false;
    };
  }, [id, isEdit]);

  const setField = (field: keyof FormState, value: string) => setForm((prev) => ({ ...prev, [field]: value }));

  const toggleItem = (field: "eventTypeIds" | "categoryIds", value: string) => {
    setForm((prev) => {
      const current = prev[field];
      const next = current.includes(value) ? current.filter((v) => v !== value) : [...current, value];
      return { ...prev, [field]: next };
    });
  };

  const validate = (): string[] => {
    const errors: string[] = [];
    if (!form.title.trim()) errors.push("El título es obligatorio.");
    if (!form.description.trim()) errors.push("La descripción es obligatoria.");
    if (!form.startDate) errors.push("La fecha de inicio es obligatoria.");
    if (!form.endDate) errors.push("La fecha de fin es obligatoria.");
    if (form.startDate && form.endDate && new Date(form.endDate) <= new Date(form.startDate)) {
      errors.push("La fecha de fin debe ser posterior a la fecha de inicio.");
    }
    if (!form.locationName.trim()) errors.push("El lugar es obligatorio.");
    if (!form.address.trim()) errors.push("La dirección es obligatoria.");
    if (!form.localityId) errors.push("Selecciona una localidad.");
    if (!form.maxParticipants || Number(form.maxParticipants) <= 0) errors.push("El cupo máximo debe ser mayor a 0.");
    if (!form.maxParticipantsPerGroup || Number(form.maxParticipantsPerGroup) <= 0) {
      errors.push("El máximo por grupo debe ser mayor a 0.");
    }
    if (form.eventTypeIds.length === 0) errors.push("Selecciona al menos un tipo de evento.");
    if (form.categoryIds.length === 0) errors.push("Selecciona al menos una categoría.");
    return errors;
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (isPast) return;
    setError("");
    const errors = validate();
    if (errors.length > 0) {
      setError(errors.join(" "));
      return;
    }
    setSaving(true);
    const payload = {
      title: form.title.trim(),
      description: form.description.trim(),
      imageUrl: form.imageUrl.trim() || null,
      startDate: form.startDate,
      endDate: form.endDate,
      locationName: form.locationName.trim(),
      address: form.address.trim(),
      localityId: Number(form.localityId),
      maxParticipants: Number(form.maxParticipants),
      maxParticipantsPerGroup: Number(form.maxParticipantsPerGroup),
      eventTypeIds: form.eventTypeIds.map(Number),
      categoryIds: form.categoryIds.map(Number),
    };
    try {
      if (isEdit) {
        await eventsApi.update({ ...payload, id: Number(id) });
      } else {
        await eventsApi.create(payload);
      }
      navigate("/admin/events");
    } catch (err) {
      setError(extractErrorMessage(err));
    } finally {
      setSaving(false);
    }
  };

  if (loading) return <PageLoader />;

  return (
    <div className="container page page-narrow">
      <Link to="/admin/events" className="btn btn-ghost btn-sm">
        ← Volver
      </Link>
      <h2 className="page-title">{isEdit ? "Editar evento" : "Nuevo evento"}</h2>

      {isPast && (
        <Alert tone="warning">
          Este evento ya finalizó. La información se muestra en modo de solo lectura para mantener la
          transparencia del registro histórico.
        </Alert>
      )}
      {error && <Alert tone="error">{error}</Alert>}

      <form onSubmit={handleSubmit} className="form">
        <fieldset disabled={isPast} style={{ border: "none", padding: 0, margin: 0 }}>
        <Card title="Información general">
          <Input label="Título" name="title" value={form.title} onChange={(e) => setField("title", e.target.value)} placeholder="Festival de Danza Folclórica" required={!isPast} />
          <Textarea
            label="Descripción"
            name="description"
            rows={5}
            value={form.description}
            onChange={(e) => setField("description", e.target.value)}
            placeholder="Describe el evento, la convocatoria y los requisitos para participar..."
            required={!isPast}
          />
          <Input
            label="URL de imagen (opcional)"
            name="imageUrl"
            value={form.imageUrl}
            onChange={(e) => setField("imageUrl", e.target.value)}
            placeholder="https://..."
            hint="Puedes dejar vacío si no tienes una imagen externa."
          />
        </Card>

        <Card title="Fecha y lugar">
          <div className="form-row">
            <Input label="Fecha de inicio" name="startDate" type="datetime-local" value={form.startDate} onChange={(e) => setField("startDate", e.target.value)} required={!isPast} />
            <Input label="Fecha de fin" name="endDate" type="datetime-local" value={form.endDate} onChange={(e) => setField("endDate", e.target.value)} required={!isPast} />
          </div>
          <div className="form-row">
            <Input label="Nombre del lugar" name="locationName" value={form.locationName} onChange={(e) => setField("locationName", e.target.value)} placeholder="Teatro Municipal" required={!isPast} />
            <Input label="Dirección" name="address" value={form.address} onChange={(e) => setField("address", e.target.value)} placeholder="Calle 10 # 5-32" required={!isPast} />
          </div>
          <Select label="Localidad" name="localityId" value={form.localityId} onChange={(e) => setField("localityId", e.target.value)}>
            <option value="">Selecciona una localidad</option>
            {localities.map((loc) => (
              <option key={loc.id} value={loc.id}>
                {loc.name}
              </option>
            ))}
          </Select>
        </Card>

        <Card title="Capacidad">
          <div className="form-row">
            <Input
              label="Cupo máximo total"
              name="maxParticipants"
              type="number"
              min={1}
              value={form.maxParticipants}
              onChange={(e) => setField("maxParticipants", e.target.value)}
              required={!isPast}
            />
            <Input
              label="Máximo por grupo"
              name="maxParticipantsPerGroup"
              type="number"
              min={1}
              value={form.maxParticipantsPerGroup}
              onChange={(e) => setField("maxParticipantsPerGroup", e.target.value)}
              required={!isPast}
            />
          </div>
        </Card>

        <Card title="Tipos y categorías">
          <div className="field">
            <label>Tipos de evento</label>
            <div className="chip-group">
              {eventTypes.map((type) => (
                <button
                  key={type.id}
                  type="button"
                  className={`chip ${form.eventTypeIds.includes(String(type.id)) ? "chip-active" : ""}`}
                  onClick={() => !isPast && toggleItem("eventTypeIds", String(type.id))}
                >
                  {type.name}
                </button>
              ))}
            </div>
          </div>
          <div className="field">
            <label>Categorías</label>
            <div className="chip-group">
              {categories.map((cat) => (
                <button
                  key={cat.id}
                  type="button"
                  className={`chip ${form.categoryIds.includes(String(cat.id)) ? "chip-active" : ""}`}
                  onClick={() => !isPast && toggleItem("categoryIds", String(cat.id))}
                >
                  {cat.name}
                </button>
              ))}
            </div>
          </div>
        </Card>
        </fieldset>

        <div className="form-actions">
          <Button type="button" variant="ghost" onClick={() => navigate(-1)}>
            {isPast ? "Volver" : "Cancelar"}
          </Button>
          {!isPast && (
            <Button type="submit" loading={saving}>
              {isEdit ? "Guardar cambios" : "Crear evento"}
            </Button>
          )}
        </div>
      </form>
    </div>
  );
}
