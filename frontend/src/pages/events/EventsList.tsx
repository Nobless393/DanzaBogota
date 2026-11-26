import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { eventsApi, type EventQueryParams } from "../../api/events";
import { catalogApi } from "../../api/catalog";
import { extractErrorMessage } from "../../api/client";
import type { EventDto, LocalityDto, EventTypeDto } from "../../types";
import {
  Alert,
  Card,
  EmptyState,
  PageLoader,
  Pagination,
  PageHeader,
  Select,
  StatusBadge,
} from "../../components/ui";
import { formatDate } from "../../utils/format";

const PAGE_SIZE = 9;

export default function EventsList() {
  const [events, setEvents] = useState<EventDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalItems, setTotalItems] = useState(0);

  // filtros
  const [localities, setLocalities] = useState<LocalityDto[]>([]);
  const [eventTypes, setEventTypes] = useState<EventTypeDto[]>([]);
  const [localityId, setLocalityId] = useState("");
  const [eventTypeId, setEventTypeId] = useState("");
  const [search, setSearch] = useState("");
  const [searchInput, setSearchInput] = useState("");

  // cargar catálogos una sola vez
  useEffect(() => {
    Promise.all([catalogApi.getLocalities(), catalogApi.getEventTypes()])
      .then(([locs, types]) => {
        setLocalities(locs);
        setEventTypes(types);
      })
      .catch(() => {
        // catálogos no críticos — se ignora error silenciosamente
      });
  }, []);

  // cargar eventos cuando cambian filtros o página
  useEffect(() => {
    setLoading(true);
    setError("");
    const params: EventQueryParams = {
      page,
      pageSize: PAGE_SIZE,
      localityId: localityId ? Number(localityId) : undefined,
      eventTypeId: eventTypeId ? Number(eventTypeId) : undefined,
      search: search || undefined,
    };
    eventsApi
      .getPublished(params)
      .then((data) => {
        setEvents(data.items);
        setTotalPages(data.totalPages);
        setTotalItems(data.totalItems);
      })
      .catch((err) => setError(extractErrorMessage(err)))
      .finally(() => setLoading(false));
  }, [page, localityId, eventTypeId, search]);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    setPage(1);
    setSearch(searchInput);
  };

  const clearFilters = () => {
    setLocalityId("");
    setEventTypeId("");
    setSearch("");
    setSearchInput("");
    setPage(1);
  };

  const hasFilters = localityId || eventTypeId || search;

  return (
    <div className="container page">
      <PageHeader
        title="Eventos culturales"
        subtitle="Convocatorias y muestras de danza en las localidades de Bogotá."
      />

      {/* Barra de búsqueda y filtros */}
      <div className="toolbar toolbar-wrap">
        <form onSubmit={handleSearch} className="search-form">
          <input
            className="input"
            type="text"
            placeholder="Buscar evento..."
            value={searchInput}
            onChange={(e) => setSearchInput(e.target.value)}
          />
          <button type="submit" className="btn btn-primary btn-sm">
            Buscar
          </button>
        </form>

        <Select
          value={localityId}
          onChange={(e) => { setLocalityId(e.target.value); setPage(1); }}
        >
          <option value="">Todas las localidades</option>
          {localities.map((loc) => (
            <option key={loc.id} value={loc.id}>
              {loc.name}
            </option>
          ))}
        </Select>

        <Select
          value={eventTypeId}
          onChange={(e) => { setEventTypeId(e.target.value); setPage(1); }}
        >
          <option value="">Todos los tipos</option>
          {eventTypes.map((type) => (
            <option key={type.id} value={type.id}>
              {type.name}
            </option>
          ))}
        </Select>

        {hasFilters && (
          <button type="button" className="btn btn-ghost btn-sm" onClick={clearFilters}>
            Limpiar filtros
          </button>
        )}
      </div>

      {error && <Alert tone="error">{error}</Alert>}

      {loading ? (
        <PageLoader />
      ) : events.length === 0 ? (
        <EmptyState
          title="No se encontraron eventos"
          message={
            hasFilters
              ? "Intenta con otros filtros o limpia la búsqueda."
              : "Aún no hay eventos publicados. Vuelve pronto."
          }
        />
      ) : (
        <>
          <div className="event-grid">
            {events.map((event) => (
              <Link to={`/events/${event.id}`} key={event.id} className="event-card-link">
                <Card className="event-card">
                  {event.imageUrl && (
                    <img
                      src={event.imageUrl}
                      alt={event.title}
                      className="event-card-image"
                      loading="lazy"
                    />
                  )}
                  <div className="event-card-body">
                    <div className="event-card-meta">
                      <span>{event.localityName}</span>
                      <StatusBadge status={event.status} />
                    </div>
                    <h3>{event.title}</h3>
                    <p className="event-card-date">{formatDate(event.startDate)}</p>
                    <p className="event-card-location">
                      {event.locationName}
                      {event.address ? ` — ${event.address}` : ""}
                    </p>
                    {event.maxParticipants != null && (
                      <p className="event-card-slots">
                        Cupo: {event.maxParticipants} participantes
                      </p>
                    )}
                  </div>
                </Card>
              </Link>
            ))}
          </div>
          <Pagination
            page={page}
            totalPages={totalPages}
            totalItems={totalItems}
            onChange={setPage}
          />
        </>
      )}
    </div>
  );
}
