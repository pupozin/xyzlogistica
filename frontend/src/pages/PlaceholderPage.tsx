type PlaceholderPageProps = {
  title: string
}

function PlaceholderPage({ title }: PlaceholderPageProps) {
  return <section className="page-placeholder" aria-label={title} />
}

export default PlaceholderPage
